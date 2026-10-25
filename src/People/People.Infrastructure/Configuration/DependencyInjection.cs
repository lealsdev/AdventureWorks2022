using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using People.Application.Abstractions;
using People.Infrastructure.Persistence;

namespace People.Infrastructure.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PersonDbContext>(options =>
            {
                options.UseSqlServer(GetConnectionString(configuration));
            });

            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<PersonDbContext>());

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
