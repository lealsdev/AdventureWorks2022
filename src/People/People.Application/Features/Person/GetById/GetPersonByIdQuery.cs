using MediatR;

namespace People.Application.Features.Person.GetById
{
    public class GetPersonByIdQuery : IRequest<GetPersonByIdQueryResult>
    {
        public int BusinessEntityId { get; }

        public GetPersonByIdQuery(int businessEntityId)
        {
            BusinessEntityId = businessEntityId;
        }
    }
}
