using BusinessEntity.Application.Abstractions.Persistence;
using BusinessEntity.Infrastructure.Persistence;
using BusinessEntity.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessEntity.Infrastructure.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<BusinessEntityDbContext>(options =>
            {
                options.UseSqlServer(GetConnectionString(configuration));
            });

            services.AddScoped<IBusinessEntityRepository, BusinessEntityRepository>();
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<BusinessEntityDbContext>());
            
            return services;
        }

        private static string GetConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("AdventureWorks");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'AdventureWorks' not found.");
            }
            return connectionString;
        }
    }
}
