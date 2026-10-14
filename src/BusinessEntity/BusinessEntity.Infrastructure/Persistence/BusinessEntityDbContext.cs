using BusinessEntity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessEntity.Infrastructure.Persistence
{
    public sealed class BusinessEntityDbContext(
    DbContextOptions<BusinessEntityDbContext> options) : DbContext(options)
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
