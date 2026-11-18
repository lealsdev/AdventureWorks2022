using People.Application.Features.Person.GetById;
using People.Domain.Entities;
using People.Domain.Enums;
using People.Domain.ValueObjects.AdditionalContactInfo;
using People.Domain.ValueObjects.Demographics;
namespace People.UnitTests;

internal static class TestPerson
{
    internal static GetPersonByIdQueryResult Result() => new(42, PersonType.IndividualCustomer, true, "Ms", "Ana", "M", "Silva", "Jr",
        EmailPromotionPreference.AdventureWorksAndPartners, new ContactInformation([]), new PersonDemographics { TotalChildren = 2 },
        Guid.Parse("8e1e7010-aeb7-4329-9b43-a727241d149a"), new DateTime(2026, 1, 2));

    internal static PersonEntity Entity(GetPersonByIdQueryResult result)
    {
        var entity = (PersonEntity)Activator.CreateInstance(typeof(PersonEntity), nonPublic: true)!;
        foreach (var property in typeof(GetPersonByIdQueryResult).GetProperties())
            typeof(PersonEntity).GetProperty(property.Name)!.SetValue(entity, property.GetValue(result));
        return entity;
    }
}
