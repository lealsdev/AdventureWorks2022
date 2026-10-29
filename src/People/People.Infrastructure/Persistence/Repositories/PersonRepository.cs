using Microsoft.EntityFrameworkCore;
using People.Application.Abstractions;
using People.Domain.Entities;

namespace People.Infrastructure.Persistence.Repositories
{
    public sealed class PersonRepository(PersonDbContext context) : IPersonRepository
    {
        public Task<PersonEntity?> GetByIdAsync(int businessEntityId, CancellationToken cancellationToken = default)
        {
            return context.People
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.BusinessEntityId == businessEntityId,
                    cancellationToken);
        }
    }
}
