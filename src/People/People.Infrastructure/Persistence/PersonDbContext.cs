using Microsoft.EntityFrameworkCore;
using People.Application.Abstractions;
using People.Domain.Entities;

namespace People.Infrastructure.Persistence
{
    public sealed class PersonDbContext(
    DbContextOptions<PersonDbContext> options) : DbContext(options), IUnitOfWork
    {
        public DbSet<PersonEntity> People =>
            Set<PersonEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(PersonDbContext).Assembly);
        }
    }
}
