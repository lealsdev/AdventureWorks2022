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
        int EmailPromotion,
        string? AdditionalContactInfo,
        string? Demographics,
        Guid RowGuid,
        DateTime ModifiedDate
   );
}
