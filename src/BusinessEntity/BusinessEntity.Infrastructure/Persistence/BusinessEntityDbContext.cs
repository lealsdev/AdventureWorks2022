using BusinessEntity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using BusinessEntity.Application.Abstractions.Persistence;

namespace BusinessEntity.Infrastructure.Persistence
{
    public sealed class BusinessEntityDbContext(
    DbContextOptions<BusinessEntityDbContext> options) : DbContext(options), IUnitOfWork
    {
        public DbSet<BusinessEntityEntity> BusinessEntities =>
            Set<BusinessEntityEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(BusinessEntityDbContext).Assembly);
        }
    }
}
