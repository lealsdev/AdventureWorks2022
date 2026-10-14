using Asp.Versioning;
using BusinessEntity.Api.Configurations;

namespace BusinessEntity.Api.Extensions;

public static class ApiVersioningExtensions
{
    public static IServiceCollection AddApiVersioningConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = configuration
            .GetRequiredSection(ApiVersioningSettings.SectionName)
            .Get<ApiVersioningSettings>()
            ?? throw new InvalidOperationException(
                "Invalid API versioning settings.");

        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(
                settings.MajorVersion,
                settings.MinorVersion,
                settings.Status);

            options.AssumeDefaultVersionWhenUnspecified =
                settings.AssumeDefaultVersionWhenUnspecified;

            options.ReportApiVersions =
                settings.ReportApiVersions;

            options.ApiVersionReader =
                new HeaderApiVersionReader(settings.HeaderName);
        })
        .AddMvc();

        return services;
    }
}