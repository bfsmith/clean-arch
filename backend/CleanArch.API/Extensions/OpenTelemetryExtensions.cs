using System.Linq;
using CleanArch.API.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace CleanArch.API.Extensions;

/// <summary>
/// Extension methods for configuring OpenTelemetry observability.
/// </summary>
public static class OpenTelemetryExtensions
{
    /// <summary>
    /// Adds OpenTelemetry instrumentation for tracing, metrics, and logging to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add OpenTelemetry to.</param>
    /// <param name="options">The OpenTelemetry configuration options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddOpenTelemetry(
        this IServiceCollection services,
        OpenTelemetryOptions options)
    {
        var otlpEndpoint = options.OtlpEndpoint;
        var requestPathsToIgnore = options.RequestPathsToIgnore;
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        // Configure resource attributes
        var attributes = new Dictionary<string, object> { ["deployment.environment"] = environment, };

        var resourceBuilder = ResourceBuilder
            .CreateDefault()
            .AddService(
                serviceName: options.ServiceName,
                serviceNamespace: options.Namespace,
                serviceVersion: options.ServiceVersion,
                !string.IsNullOrWhiteSpace(options.InstanceId),
                string.IsNullOrWhiteSpace(options.InstanceId) ? null : options.InstanceId)
            .AddAttributes(attributes);

        // Configure Tracing
        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.EnrichWithHttpRequest = (activity, request) =>
                        {
                            activity.SetTag("http.request.method", request.Method);
                            activity.SetTag("http.request.path", request.Path.Value);
                        };
                        options.EnrichWithHttpResponse = (activity, response) =>
                        {
                            activity.SetTag("http.response.status_code", response.StatusCode);
                        };
                    })
                    .AddHttpClientInstrumentation(httpClientOptions =>
                    {
                        httpClientOptions.RecordException = true;
                        if (requestPathsToIgnore.Count > 0)
                        {
                            httpClientOptions.FilterHttpRequestMessage = (request) =>
                            {
                                var requestPath = request.RequestUri?.AbsolutePath ?? string.Empty;
                                return !requestPathsToIgnore.Any(path =>
                                    requestPath.StartsWith(path, StringComparison.OrdinalIgnoreCase));
                            };
                        }
                    });
                builder.AddOtlpExporter(options =>
                {
                    if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                    {
                        options.Endpoint = new Uri(otlpEndpoint);
                        // Aspire dashboard expects gRPC protocol for traces
                        options.Protocol = OtlpExportProtocol.Grpc;
                    }
                });
            })
            .WithMetrics(builder =>
            {
                builder
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                        {
                            options.Endpoint = new Uri(otlpEndpoint);
                        }
                    });
            })
            .WithLogging(builder =>
            {
                builder.SetResourceBuilder(resourceBuilder);
                builder.AddOtlpExporter(options =>
                {
                    if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                    {
                        options.Endpoint = new Uri(otlpEndpoint);
                    }
                });
            });

        return services;
    }
}
