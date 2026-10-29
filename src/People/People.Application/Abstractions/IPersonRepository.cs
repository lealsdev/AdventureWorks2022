using People.Domain.Entities;

namespace People.Application.Abstractions
{
    public interface IPersonRepository
    {
        Task<PersonEntity?> GetByIdAsync(int businessEntityId, CancellationToken cancellationToken = default);
    }
}
