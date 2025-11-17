using Microsoft.Extensions.Configuration;

namespace CleanArch.API.Configuration;

/// <summary>
/// Configuration options for OpenTelemetry observability.
/// </summary>
public class OpenTelemetryOptions
{
    /// <summary>
    /// The configuration section name for OpenTelemetry settings.
    /// </summary>
    public const string SectionName = "OpenTelemetry";

    /// <summary>
    /// Gets or sets the OTLP (OpenTelemetry Protocol) endpoint URL for exporting telemetry data.
    /// Required. Example: "http://localhost:18888" for the Aspire dashboard.
    /// </summary>
    public string OtlpEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the service name used to identify this service in telemetry data.
    /// Required. This value appears in traces, metrics, and logs.
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the service version used to identify the version of this service in telemetry data.
    /// Required. This value appears in traces, metrics, and logs.
    /// </summary>
    public string ServiceVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the namespace used to organize services in telemetry data.
    /// Defaults to "default" if not specified.
    /// </summary>
    public string Namespace { get; set; } = "default";

    /// <summary>
    /// Gets or sets the instance ID used to uniquely identify this service instance in telemetry data.
    /// Optional. If not specified, the instance ID will not be included in telemetry data.
    /// </summary>
    public string? InstanceId { get; set; }

    /// <summary>
    /// Gets or sets the list of request paths that should be ignored by HTTP client instrumentation.
    /// Optional. If not specified, all requests will be captured.
    /// </summary>
    public List<string> RequestPathsToIgnore { get; set; } = new();
}

/// <summary>
/// Extension methods for loading <see cref="OpenTelemetryOptions"/> from configuration.
/// </summary>
public static class OpenTelemetryOptionsExtensions
{
    /// <summary>
    /// Loads and validates OpenTelemetry configuration options from the configuration section.
    /// </summary>
    /// <param name="configuration">The configuration manager to read from.</param>
    /// <returns>A validated <see cref="OpenTelemetryOptions"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required configuration values are missing or invalid.</exception>
    public static OpenTelemetryOptions LoadOpenTelemetryOptions(this IConfigurationManager configuration)
    {
        var section = configuration.GetSection(OpenTelemetryOptions.SectionName);
        var options = section.Get<OpenTelemetryOptions>() ?? new OpenTelemetryOptions();

        // Fallback to standard OpenTelemetry environment variable if OtlpEndpoint is not configured
        if (string.IsNullOrWhiteSpace(options.OtlpEndpoint))
        {
            options.OtlpEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT") ?? string.Empty;
        }

        // Validate required properties
        if (string.IsNullOrWhiteSpace(options.ServiceName))
        {
            throw new InvalidOperationException($"{OpenTelemetryOptions.SectionName}:{nameof(OpenTelemetryOptions.ServiceName)} is required.");
        }

        if (string.IsNullOrWhiteSpace(options.ServiceVersion))
        {
            throw new InvalidOperationException($"{OpenTelemetryOptions.SectionName}:{nameof(OpenTelemetryOptions.ServiceVersion)} is required.");
        }

        return options;
    }
}

