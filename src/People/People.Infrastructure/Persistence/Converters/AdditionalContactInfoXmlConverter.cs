using System.Collections.Immutable;
using System.Xml.Linq;
using People.Domain.ValueObjects.AdditionalContactInfo;
namespace People.Infrastructure.Persistence.Converters;

public static class AdditionalContactInfoXmlConverter
{
    private static readonly XNamespace Info = PersonXml.Base + "ContactInfo";
    private static readonly XNamespace Types = PersonXml.Base + "ContactTypes";
    private static readonly XNamespace Records = PersonXml.Base + "ContactRecord";
    private static PhoneKind ToPhoneKind(string name) => name switch
    {
        "telephoneNumber" => PhoneKind.Telephone,
        "mobile" => PhoneKind.Mobile,
        "pager" => PhoneKind.Pager,
        "facsimileTelephoneNumber" => PhoneKind.Fax,
        "telexNumber" => PhoneKind.Telex,
        "internationaliSDNNumber" => PhoneKind.InternationalIsdn,
        _ => throw new NotSupportedException($"Unsupported phone type: {name}")
    };

    private static string FromPhoneKind(PhoneKind kind) => kind switch
    {
        PhoneKind.Telephone => "telephoneNumber",
        PhoneKind.Mobile => "mobile",
        PhoneKind.Pager => "pager",
        PhoneKind.Fax => "facsimileTelephoneNumber",
        PhoneKind.Telex => "telexNumber",
        PhoneKind.InternationalIsdn => "internationaliSDNNumber",
        _ => throw new ArgumentOutOfRangeException(nameof(kind))
    };

    private static AddressKind ToAddressKind(string name) => name switch
    {
        "homePostalAddress" => AddressKind.Home,
        "physicalDeliveryOfficeName" => AddressKind.DeliveryOffice,
        "registeredAddress" => AddressKind.Registered,
        _ => throw new NotSupportedException($"Unsupported address type: {name}")
    };

    private static string FromAddressKind(AddressKind kind) => kind switch
    {
        AddressKind.Home => "homePostalAddress",
        AddressKind.DeliveryOffice => "physicalDeliveryOfficeName",
        AddressKind.Registered => "registeredAddress",
        _ => throw new ArgumentOutOfRangeException(nameof(kind))
    };

    public static ContactInformation? FromXml(string? xml) => 
        xml is null 
        ? null 
        : new(ReadContent(PersonXml.Read(xml, Info + "AdditionalContactInfo")));

    public static string? ToXml(ContactInformation? value) => value is null ? null :
        PersonXml.Write(new XElement(Info + "AdditionalContactInfo", WriteContent(value.Content)));

    private static ImmutableArray<ContactContent> ReadContent(XElement parent) => parent.Nodes().Select(
        node => node switch
        {
            XText text => new ContactContent(text: text.Value),
            XElement element => ReadElement(element),
            _ => throw new NotSupportedException("Unsupported contact content.")
        }).ToImmutableArray();

    private static ContactContent ReadElement(XElement element)
    {
        var name = element.Name.LocalName;

        if (element.Name == Records + "ContactRecord")
            return new(record: new(ReadContent(element), element.Attribute("date") is { } date ? PersonXml.Date(date.Value) : null));

        if (element.Name.Namespace != Types)
            throw new NotSupportedException($"Unsupported contact: {element.Name}");

        ContactInstructions? Instructions() => element.Element(Types + "SpecialInstructions") is { } instructions
            ? new(ReadContent(instructions))
            : null;

        string Required(string child) => element.Element(Types + child)!.Value;

        if (element.Name.Namespace == Types &&
            name is "telephoneNumber" 
                or "mobile" 
                or "pager"
                or "facsimileTelephoneNumber" 
                or "telexNumber"
                or "internationaliSDNNumber")
        {
            return new(
                phone: new(
                    ToPhoneKind(name),
                    Required("number"),
                    Instructions()));
        }

        if (name == "eMail")
            return new(email: new(Required("eMailAddress"), Instructions()));

        if (name is "homePostalAddress"
            or "physicalDeliveryOfficeName"
            or "registeredAddress")
        {
            return new(
                address: new(
                    ToAddressKind(name),
                    element.Elements(Types + "Street")
                        .Select(e => e.Value)
                        .ToImmutableArray(),
                    Required("City"),
                    Required("StateProvince"),
                    (string?)element.Element(Types + "PostalCode"),
                    Required("CountryRegion"),
                    Instructions()));
        }

        throw new NotSupportedException($"Unsupported contact: {element.Name}");
    }
    private static IEnumerable<object> WriteContent(ImmutableArray<ContactContent> content)
    {
        foreach (var item in content)
        {
            if (item.Text is { } text) 
            { 
                yield return new XText(text); 
                continue; 
            }

            if (item.Phone is { } phone)
                yield return new XElement(Types + FromPhoneKind(phone.Kind), new XElement(Types + "number", phone.Number), Instructions(phone.Instructions));
            else if (item.Email is { } email)
                yield return new XElement(Types + "eMail", new XElement(Types + "eMailAddress", email.Address), Instructions(email.Instructions));
            else if (item.Address is { } address)
                yield return new XElement(Types + FromAddressKind(address.Kind), address.Streets.Select(s => new XElement(Types + "Street", s)),
                    new XElement(Types + "City", address.City), new XElement(Types + "StateProvince", address.StateProvince),
                    address.PostalCode is null ? null : new XElement(Types + "PostalCode", address.PostalCode),
                    new XElement(Types + "CountryRegion", address.CountryRegion), Instructions(address.Instructions));
            else if (item.Record is { } record)
                yield return new XElement(Records + "ContactRecord", record.Date is null ? null : new XAttribute("date", PersonXml.Date(record.Date)), WriteContent(record.Content));
        }
    }
    private static XElement? Instructions(ContactInstructions? value) => value is null ? null : new(Types + "SpecialInstructions", WriteContent(value.Content));
}
