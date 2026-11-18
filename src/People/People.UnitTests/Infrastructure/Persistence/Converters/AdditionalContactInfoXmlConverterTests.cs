using System.Collections.Immutable;
using System.Xml.Linq;
using People.Domain.ValueObjects.AdditionalContactInfo;
using People.Infrastructure.Persistence.Converters;

namespace People.UnitTests.Infrastructure.Persistence.Converters;

public sealed class AdditionalContactInfoXmlConverterTests
{
    private const string Info = "http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ContactInfo";
    private const string Types = "http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ContactTypes";
    private const string Records = "http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ContactRecord";
    private static string Wrap(string body) => $"<AdditionalContactInfo xmlns=\"{Info}\" xmlns:t=\"{Types}\" xmlns:r=\"{Records}\">{body}</AdditionalContactInfo>";

    [Fact]
    public void Nulls_ArePreserved()
    {
        Assert.Null(AdditionalContactInfoXmlConverter.FromXml(null));
        Assert.Null(AdditionalContactInfoXmlConverter.ToXml(null));
    }

    [Theory]
    [InlineData("")]
    [InlineData("free text &amp; more")]
    [InlineData("<t:telephoneNumber><t:number>123-456</t:number></t:telephoneNumber>")]
    [InlineData("<t:eMail><t:eMailAddress>a@example.com</t:eMailAddress></t:eMail><t:eMail><t:eMailAddress>b@example.com</t:eMailAddress></t:eMail>")]
    [InlineData("<t:homePostalAddress><t:Street>A</t:Street><t:Street>B</t:Street><t:City>C</t:City><t:StateProvince>S</t:StateProvince><t:CountryRegion>P</t:CountryRegion></t:homePostalAddress>")]
    [InlineData("before<r:ContactRecord date=\"2022-01-02+05:30\">called<t:mobile><t:number>123</t:number><t:SpecialInstructions>use <t:telephoneNumber><t:number>456</t:number></t:telephoneNumber> after lunch</t:SpecialInstructions></t:mobile>end</r:ContactRecord>after")]
    public void ReadWrite_PreservesSemanticContentAndOrder(string body)
    {
        var xml = Wrap(body);
        var value = AdditionalContactInfoXmlConverter.FromXml(xml)!;
        var output = AdditionalContactInfoXmlConverter.ToXml(value)!;
        var original = XElement.Parse(xml, LoadOptions.PreserveWhitespace);
        var generated = XElement.Parse(output, LoadOptions.PreserveWhitespace);
        
        original.DescendantsAndSelf().Attributes().Where(a => a.IsNamespaceDeclaration).Remove();
        generated.DescendantsAndSelf().Attributes().Where(a => a.IsNamespaceDeclaration).Remove();

        foreach (var element in original.DescendantsAndSelf().Concat(generated.DescendantsAndSelf()))
            if (!element.Nodes().Any()) 
                element.Value = "";

        Assert.True(XNode.DeepEquals(original, generated));
        Assert.Equal(output, AdditionalContactInfoXmlConverter.ToXml(AdditionalContactInfoXmlConverter.FromXml(output)));
    }

    [Theory]
    [InlineData("telephoneNumber", PhoneKind.Telephone)]
    [InlineData("mobile", PhoneKind.Mobile)]
    [InlineData("pager", PhoneKind.Pager)]
    [InlineData("facsimileTelephoneNumber", PhoneKind.Fax)]
    [InlineData("telexNumber", PhoneKind.Telex)]
    [InlineData("internationaliSDNNumber", PhoneKind.InternationalIsdn)]
    public void PhoneKinds_AreTypedAndWritable(string name, PhoneKind kind)
    {
        var value = new ContactInformation([new(phone: new(kind, "(123)-456"))]);
        var xml = AdditionalContactInfoXmlConverter.ToXml(value)!;

        Assert.Equal(name, XElement.Parse(xml).Elements().Single().Name.LocalName);
        Assert.Equal(value.Content[0].Phone, AdditionalContactInfoXmlConverter.FromXml(xml)!.Content[0].Phone);
    }

    [Theory]
    [InlineData(AddressKind.Home)]
    [InlineData(AddressKind.DeliveryOffice)]
    [InlineData(AddressKind.Registered)]
    public void Addresses_PreserveAllFields(AddressKind kind)
    {
        var address = new ContactAddress(kind, ["one", "two"], "city", "state", "123", "country", new([new(text: "leave here")]));
        var value = AdditionalContactInfoXmlConverter.FromXml(AdditionalContactInfoXmlConverter.ToXml(new([new(address: address)])))!.Content[0].Address!;

        Assert.Equal(address.Streets, value.Streets.ToArray());
        Assert.Equal(address.City, value.City);
        Assert.Equal(address.StateProvince, value.StateProvince);
        Assert.Equal(address.PostalCode, value.PostalCode);
        Assert.Equal(address.CountryRegion, value.CountryRegion);
        Assert.Equal("leave here", value.Instructions!.Content[0].Text);
    }

    [Theory]
    [InlineData("<broken")]
    [InlineData("<AdditionalContactInfo/>")]
    [InlineData("")]
    public void InvalidDocuments_Throw(string xml) => Assert.ThrowsAny<Exception>(() => AdditionalContactInfoXmlConverter.FromXml(xml));

    [Theory]
    [InlineData("<t:unknown/>")]
    [InlineData("<t:telephoneNumber/>")]
    [InlineData("<t:telephoneNumber><t:number>abc</t:number></t:telephoneNumber>")]
    [InlineData("<r:ContactRecord date=\"2022-02-30\"/>")]
    [InlineData("<t:eMail unexpected=\"x\"><t:eMailAddress>a</t:eMailAddress></t:eMail>")]
    public void InvalidContent_ThrowsWithoutLosingData(string body) => Assert.ThrowsAny<Exception>(() => AdditionalContactInfoXmlConverter.FromXml(Wrap(body)));

    [Fact]
    public void NestedContent_ExposesTypedValuesAndDate()
    {
        var xml = Wrap("before<r:ContactRecord date=\"2022-01-02Z\">called<t:eMail><t:eMailAddress>a@example.com</t:eMailAddress><t:SpecialInstructions>call<t:mobile><t:number>123</t:number></t:mobile>later</t:SpecialInstructions></t:eMail>end</r:ContactRecord>after");
        var content = AdditionalContactInfoXmlConverter.FromXml(xml)!.Content;
        Assert.Equal("before", content[0].Text);
        Assert.Equal("after", content[2].Text);

        var record = content[1].Record!;
        Assert.Equal(new DateOnly(2022, 1, 2), record.Date!.Value);
        Assert.Equal(0, record.Date.OffsetMinutes);
        Assert.Equal("called", record.Content[0].Text);
        Assert.Equal("end", record.Content[2].Text);

        var email = record.Content[1].Email!;
        Assert.Equal("a@example.com", email.Address);
        Assert.Equal("call", email.Instructions!.Content[0].Text);
        Assert.Equal(PhoneKind.Mobile, email.Instructions.Content[1].Phone!.Kind);
        Assert.Equal("123", email.Instructions.Content[1].Phone!.Number);
        Assert.Equal("later", email.Instructions.Content[2].Text);
    }

    [Theory]
    [InlineData("<!-- comment -->")]
    [InlineData("<?instruction value?>")]
    [InlineData("<r:ContactRecord xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:type=\"r:unknown\"/>")]
    public void UnrepresentableMetadata_IsRejected(string body) => Assert.ThrowsAny<Exception>(() => AdditionalContactInfoXmlConverter.FromXml(Wrap(body)));
    
    [Fact]
    public void InvalidObject_IsRejected() => Assert.ThrowsAny<Exception>(() => AdditionalContactInfoXmlConverter.ToXml(new([new(phone: new(PhoneKind.Mobile, "invalid"))])));

    [Fact]
    public void ToXml_ThrowsForUnsupportedPhoneKind()
    {
        var value = new ContactInformation(
        [
            new ContactContent(
            phone: new ContactPhone(
                (PhoneKind)999,
                "123456789"))
        ]);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => AdditionalContactInfoXmlConverter.ToXml(value));
    }

    [Fact]
    public void ToXml_ThrowsForUnsupportedAddressKind()
    {
        var value = new ContactInformation(
        [
            new ContactContent(
                address: new ContactAddress(
                    (AddressKind)999,
                    ImmutableArray<string>.Empty, "", "", "", ""))
        ]);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => AdditionalContactInfoXmlConverter.ToXml(value));
    }
}
