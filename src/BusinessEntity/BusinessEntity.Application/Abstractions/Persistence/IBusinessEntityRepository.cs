using BusinessEntity.Domain.Entities;

namespace BusinessEntity.Application.Abstractions.Persistence
{
    public interface IBusinessEntityRepository
    {
        Task<BusinessEntityEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
