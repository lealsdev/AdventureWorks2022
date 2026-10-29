using Microsoft.Extensions.DependencyInjection;
using People.Application.Common.Behaviors;

namespace People.Application.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationConfiguration(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });
            return services;
        }
    }
}
