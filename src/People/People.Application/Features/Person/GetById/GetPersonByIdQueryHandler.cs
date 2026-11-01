using MediatR;
using People.Application.Abstractions;
using People.Application.Common.Exceptions;
using People.Domain.Entities;

namespace People.Application.Features.Person.GetById
{
    public class GetPersonByIdQueryHandler : IRequestHandler<GetPersonByIdQuery, GetPersonByIdQueryResult>
    {
        private readonly IPersonRepository _repository;

        public GetPersonByIdQueryHandler(IPersonRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetPersonByIdQueryResult> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.BusinessEntityId, cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(PersonEntity));
            }

            return new GetPersonByIdQueryResult
            (
                BusinessEntityId: entity.BusinessEntityId,
                PersonType: entity.PersonType,
                NameStyle: entity.NameStyle,
                Title: entity.Title,
                FirstName: entity.FirstName,
                MiddleName: entity.MiddleName,
                LastName: entity.LastName,
                Suffix: entity.Suffix,
                EmailPromotion: entity.EmailPromotion,
                AdditionalContactInfo: entity.AdditionalContactInfo,
                Demographics: entity.Demographics,
                RowGuid: entity.RowGuid,
                ModifiedDate: entity.ModifiedDate
            );
        }
    }
}
