namespace BusinessEntity.Api.Configurations;

public sealed class ApiVersioningSettings
{
    public const string SectionName = "ApiVersioning";

    public required int MajorVersion { get; set; }

    public required int MinorVersion { get; set; }

    public string? Status { get; set; }

    public required bool AssumeDefaultVersionWhenUnspecified { get; set; }

    public required bool ReportApiVersions { get; set; }

    public required string HeaderName { get; set; }
}