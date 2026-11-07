using System.Collections.Immutable;
using System.Xml;
using System.Xml.Linq;
using People.Domain.ValueObjects.Demographics;
namespace People.Infrastructure.Persistence.Converters;

public static class DemographicsXmlConverter
{
    private static readonly XNamespace Ns = PersonXml.Base + "IndividualSurvey";
    public static PersonDemographics? FromXml(string? xml)
    {
        if (xml is null) 
            return null;

        var root = PersonXml.Read(xml, Ns + "IndividualSurvey");

        string? Get(string name) => (string?)root.Element(Ns + name);

        return new PersonDemographics
        {
            TotalPurchaseYTD = Get("TotalPurchaseYTD") is string totalPurchaseYTD ? PersonXml.Decimal(totalPurchaseYTD) : null,
            DateFirstPurchase = Get("DateFirstPurchase") is string dateFirstPurchase ? PersonXml.Date(dateFirstPurchase) : null,
            BirthDate = Get("BirthDate") is string birthDate ? PersonXml.Date(birthDate) : null,
            MaritalStatus = Get("MaritalStatus"),
            YearlyIncome = Get("YearlyIncome"),
            Gender = Get("Gender"),
            TotalChildren = Get("TotalChildren") is string totalChildren ? XmlConvert.ToInt32(totalChildren) : null,
            NumberChildrenAtHome = Get("NumberChildrenAtHome") is string numberChildrenAtHome ? XmlConvert.ToInt32(numberChildrenAtHome) : null,
            Education = Get("Education"),
            Occupation = Get("Occupation"),
            HomeOwnerFlag = Get("HomeOwnerFlag"),
            NumberCarsOwned = Get("NumberCarsOwned") is string numberCarsOwned ? XmlConvert.ToInt32(numberCarsOwned) : null,
            Hobbies = root.Elements(Ns + "Hobby").Select(e => e.Value).ToImmutableArray(),
            CommuteDistance = Get("CommuteDistance"),
            Comments = Get("Comments"),
        };
    }
    public static string? ToXml(PersonDemographics? value)
    {
        if (value is null) 
            return null;

        var root = new XElement(Ns + "IndividualSurvey");
        
        void Add(string name, object? content) 
        { 
            if (content is not null) root.Add(new XElement(Ns + name, content)); 
        }

        Add("TotalPurchaseYTD", value.TotalPurchaseYTD);
        Add("DateFirstPurchase", value.DateFirstPurchase is { } dateFirstPurchase ? PersonXml.Date(dateFirstPurchase) : null);
        Add("BirthDate", value.BirthDate is { } birthDate ? PersonXml.Date(birthDate) : null);
        Add("MaritalStatus", value.MaritalStatus);
        Add("YearlyIncome", value.YearlyIncome);
        Add("Gender", value.Gender);
        Add("TotalChildren", value.TotalChildren);
        Add("NumberChildrenAtHome", value.NumberChildrenAtHome);
        Add("Education", value.Education);
        Add("Occupation", value.Occupation);
        Add("HomeOwnerFlag", value.HomeOwnerFlag);
        Add("NumberCarsOwned", value.NumberCarsOwned);
        
        foreach (var hobby in value.Hobbies) 
            Add("Hobby", hobby);
        
        Add("CommuteDistance", value.CommuteDistance);
        Add("Comments", value.Comments);

        return PersonXml.Write(root);
    }
}
