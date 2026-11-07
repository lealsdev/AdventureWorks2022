using People.Domain.ValueObjects.AdditionalContactInfo;
using People.Domain.ValueObjects.Demographics;
using People.Domain.Enums;

namespace People.Application.Features.Person.GetById
{
    public sealed record GetPersonByIdQueryResult(
        int BusinessEntityId,
        PersonType PersonType,
        bool NameStyle,
        string? Title,
        string FirstName,
        string? MiddleName,
        string LastName,
        string? Suffix,
        EmailPromotionPreference EmailPromotion,
        ContactInformation? AdditionalContactInfo,
        PersonDemographics? Demographics,
        Guid RowGuid,
        DateTime ModifiedDate
   );
}
