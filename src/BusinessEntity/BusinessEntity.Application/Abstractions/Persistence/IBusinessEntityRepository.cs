using BusinessEntity.Domain.Entities;

namespace BusinessEntity.Application.Abstractions.Persistence
{
    public interface IBusinessEntityRepository
    {
        Task AddAsync(BusinessEntityEntity entity, CancellationToken cancellationToken = default);

        Task<BusinessEntityEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
