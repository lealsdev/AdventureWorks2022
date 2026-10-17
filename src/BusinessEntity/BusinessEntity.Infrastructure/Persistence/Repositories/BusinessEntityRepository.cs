using BusinessEntity.Application.Abstractions.Persistence;
using BusinessEntity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessEntity.Infrastructure.Persistence.Repositories
{
    public sealed class BusinessEntityRepository(
    BusinessEntityDbContext context) : IBusinessEntityRepository
    {
        public async Task AddAsync(BusinessEntityEntity entity, CancellationToken cancellationToken = default)
        {
            await context.BusinessEntities.AddAsync(entity, cancellationToken);
        }

        public Task<BusinessEntityEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return context.BusinessEntities
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.BusinessEntityId == id,
                    cancellationToken);
        }
    }
}
