using Microsoft.Extensions.DependencyInjection;

namespace BusinessEntity.Application.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationConfiguration(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
            return services;
        }
    }
}
