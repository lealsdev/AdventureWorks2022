using System.Xml.Linq;
using People.Domain.ValueObjects;
using People.Domain.ValueObjects.Demographics;
using People.Infrastructure.Persistence.Converters;

namespace People.UnitTests.Infrastructure.Persistence.Converters;

public sealed class DemographicsXmlConverterTests
{
    private const string Ns = "http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/IndividualSurvey";
    private static string Wrap(string body) => $"<IndividualSurvey xmlns=\"{Ns}\">{body}</IndividualSurvey>";

    [Fact]
    public void Nulls_ArePreserved()
    {
        Assert.Null(DemographicsXmlConverter.FromXml(null));
        Assert.Null(DemographicsXmlConverter.ToXml(null));
    }
    
    [Fact]
    public void AllFields_AreTypedAndRoundTrip()
    {
        var expected = new PersonDemographics {
            TotalPurchaseYTD = 123.45m, DateFirstPurchase = new(new(2022, 1, 2), 330), BirthDate = new(new(1990, 3, 4)),
            MaritalStatus = "S", YearlyIncome = "25001-50000", Gender = "F", TotalChildren = 2, NumberChildrenAtHome = 1,
            Education = "Degree", Occupation = "Engineer", HomeOwnerFlag = "1", NumberCarsOwned = 1,
            Hobbies = ["cycling", "music"], CommuteDistance = "1-2 Miles", Comments = "hello & goodbye"
        };

        var xml = DemographicsXmlConverter.ToXml(expected)!;

        Assert.Equal(XName.Get("IndividualSurvey", Ns), XElement.Parse(xml).Name);

        var actual = DemographicsXmlConverter.FromXml(xml)!;

        Assert.Equal(expected.Hobbies, actual.Hobbies.ToArray());
        Assert.Equal(expected with { Hobbies = actual.Hobbies }, actual);
        Assert.Equal(xml, DemographicsXmlConverter.ToXml(actual));
    }
    
    [Fact]
    public void OptionalFields_CanAllBeAbsent()
    {
        var actual = DemographicsXmlConverter.FromXml(Wrap(""))!;

        Assert.Null(actual.TotalPurchaseYTD);
        Assert.Null(actual.BirthDate);
        Assert.Empty(actual.Hobbies);
        Assert.Equal(new PersonDemographics { Hobbies = actual.Hobbies }, actual);
    }
    
    [Theory]
    [InlineData("<Hobby>a</Hobby><Hobby>a</Hobby>")]
    [InlineData("<DateFirstPurchase>2022-01-02Z</DateFirstPurchase>")]
    [InlineData("<BirthDate>2022-01-02-14:00</BirthDate>")]
    [InlineData("<TotalPurchaseYTD>-123.456</TotalPurchaseYTD><TotalChildren>-1</TotalChildren>")]
    public void ValidValues_PreserveSemanticContent(string body)
    {
        var xml = Wrap(body);

        Assert.True(XNode.DeepEquals(XElement.Parse(xml), XElement.Parse(DemographicsXmlConverter.ToXml(DemographicsXmlConverter.FromXml(xml))!)));
    }

    [Theory]
    [InlineData("2022-01-02", null)]
    [InlineData("2022-01-02Z", 0)]
    [InlineData("2022-01-02+05:30", 330)]
    [InlineData("2022-01-02-03:30", -210)]
    public void BirthDate_PreservesOptionalOffset(
    string date,
    int? expectedOffset)
    {
        var xml = Wrap($"<BirthDate>{date}</BirthDate>");

        var result = DemographicsXmlConverter.FromXml(xml)!;

        Assert.Equal(
            new DateOnly(2022, 1, 2),
            result.BirthDate!.Value);

        Assert.Equal(
            expectedOffset,
            result.BirthDate.OffsetMinutes);
    }

    [Theory]
    [InlineData("<Unknown/>")]
    [InlineData("<TotalChildren>no</TotalChildren>")]
    [InlineData("<TotalChildren>2147483648</TotalChildren>")]
    [InlineData("<BirthDate>2022-02-30</BirthDate>")]
    [InlineData("<BirthDate>2022-01-01T00:00:00</BirthDate>")]
    [InlineData("<Gender>F</Gender><Gender>M</Gender>")]
    [InlineData("<YearlyIncome>unknown</YearlyIncome>")]
    [InlineData("<CommuteDistance>unknown</CommuteDistance>")]
    [InlineData("<Comments>x</Comments><Hobby>late</Hobby>")]
    public void InvalidContent_IsRejected(string body) => Assert.ThrowsAny<Exception>(() => DemographicsXmlConverter.FromXml(Wrap(body)));
    
    [Theory]
    [InlineData("")]
    [InlineData("<broken")]
    [InlineData("<IndividualSurvey/>")]
    public void InvalidDocument_IsRejected(string xml) => Assert.ThrowsAny<Exception>(() => DemographicsXmlConverter.FromXml(xml));
    
    [Fact]
    public void ExcessDecimalPrecision_IsRejectedInsteadOfRounded() => Assert.Throws<NotSupportedException>(() =>
        DemographicsXmlConverter.FromXml(Wrap("<TotalPurchaseYTD>0.1234567890123456789012345678912345</TotalPurchaseYTD>")));
    
    [Fact]
    public void InvalidObject_IsRejected() => Assert.ThrowsAny<Exception>(() => DemographicsXmlConverter.ToXml(new() { YearlyIncome = "invalid" }));
}
