# CleanArch.OpenTelemetry

A pre-configured OpenTelemetry package for CleanArch applications that provides tracing, metrics, and logging instrumentation with OTLP export capabilities.

## Overview

This package provides a simplified way to add OpenTelemetry observability to ASP.NET Core applications. It configures tracing, metrics, and logging instrumentation with automatic resource attribute configuration and request filtering capabilities.

## Installation

Add a project reference to your API project:

```xml
<ProjectReference Include="..\CleanArch.OpenTelemetry\CleanArch.OpenTelemetry.csproj" />
```

## Quick Start

Add the following to your service configuration:

```csharp
using CleanArch.OpenTelemetry;
using CleanArch.OpenTelemetry.Configuration;

// In your service configuration
var openTelemetryOptions = configuration.LoadOpenTelemetryOptions();
services.AddOpenTelemetry(openTelemetryOptions);
```

## Configuration Reference

Configuration is provided via the `OpenTelemetry` section in `appsettings.json` or through environment variables. The `OpenTelemetryOptions` class provides the following properties:

### Required Properties

- **`ServiceName`** (string, required)
  - The service name used to identify this service in telemetry data.
  - Appears in traces, metrics, and logs.
  - Example: `"cleanarch-api"`

- **`ServiceVersion`** (string, required)
  - The service version used to identify the version of this service in telemetry data.
  - Appears in traces, metrics, and logs.
  - Example: `"1.0.0"`

### Optional Properties

- **`OtlpEndpoint`** (string, optional)
  - The OTLP (OpenTelemetry Protocol) endpoint URL for exporting telemetry data.
  - Falls back to the `OTEL_EXPORTER_OTLP_ENDPOINT` environment variable if not specified.
  - Example: `"http://localhost:4317"` (OpenTelemetry default)

- **`Namespace`** (string, optional)
  - The namespace used to organize services in telemetry data.
  - Defaults to `"default"` if not specified.

- **`InstanceId`** (string, optional)
  - The instance ID used to uniquely identify this service instance in telemetry data.
  - If not specified, the instance ID will not be included in telemetry data.

- **`RequestPathsToIgnore`** (List<string>, optional)
  - List of request paths that should be ignored by HTTP instrumentation.
  - Paths are matched using case-insensitive prefix matching.
  - If not specified, all requests will be captured.
  - Example: `["/healthz", "/metrics"]`

### Configuration Example

```json
{
  "OpenTelemetry": {
    "ServiceName": "my-api",
    "ServiceVersion": "1.0.0",
    "Namespace": "production",
    "InstanceId": "instance-001",
    "OtlpEndpoint": "http://localhost:4317",
    "RequestPathsToIgnore": [
      "/healthz",
      "/metrics"
    ]
  }
}
```

## Capabilities

This package configures the following OpenTelemetry capabilities:

### Tracing

- **ASP.NET Core Instrumentation**
  - Automatic tracing of HTTP requests
  - Exception recording enabled
  - Request/response enrichment:
    - `http.request.method` tag
    - `http.request.path` tag
    - `http.response.status_code` tag
  - Configurable request path filtering

- **HttpClient Instrumentation**
  - Automatic tracing of outgoing HTTP requests
  - Exception recording enabled
  - Configurable request path filtering

- **OTLP Exporter**
  - Exports traces via OTLP protocol
  - Configurable endpoint

### Metrics

- **ASP.NET Core Instrumentation**
  - Automatic collection of HTTP request metrics

- **HttpClient Instrumentation**
  - Automatic collection of outgoing HTTP request metrics

- **Runtime Instrumentation**
  - Automatic collection of .NET runtime metrics (GC, threading, etc.)

- **OTLP Exporter**
  - Exports metrics via OTLP protocol
  - Configurable endpoint

### Logging

- **OTLP Exporter**
  - Exports logs via OTLP protocol
  - Configurable endpoint

### Resource Attributes

The following resource attributes are automatically configured:

- `service.name` - From `ServiceName` option
- `service.namespace` - From `Namespace` option (defaults to "default")
- `service.version` - From `ServiceVersion` option
- `service.instance.id` - From `InstanceId` option (if provided)
- `deployment.environment` - From `ASPNETCORE_ENVIRONMENT` environment variable (defaults to "Development")

## Usage Examples

### Basic Setup

```csharp
using CleanArch.OpenTelemetry;
using CleanArch.OpenTelemetry.Configuration;

public class Api
{
    protected void AddOpenTelemetry()
    {
        var openTelemetryOptions = Builder.Configuration.LoadOpenTelemetryOptions();
        Builder.Services.AddOpenTelemetry(openTelemetryOptions);
    }
}
```

### Configuration via appsettings.json

```json
{
  "OpenTelemetry": {
    "ServiceName": "cleanarch-api",
    "ServiceVersion": "1.0.0",
    "RequestPathsToIgnore": [
      "/healthz"
    ]
  }
}
```

### Environment Variable Configuration

The `OtlpEndpoint` can be configured via the standard OpenTelemetry environment variable:

```bash
export OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:4317
```

If both the configuration option and environment variable are set, the configuration option takes precedence.

### Filtering Health Check Endpoints

To exclude health check endpoints from instrumentation:

```json
{
  "OpenTelemetry": {
    "ServiceName": "cleanarch-api",
    "ServiceVersion": "1.0.0",
    "RequestPathsToIgnore": [
      "/healthz",
      "/health"
    ]
  }
}
```

This will prevent health check requests from generating traces or metrics, reducing noise in your telemetry data.

## Extension Methods

### `AddOpenTelemetry(IServiceCollection, OpenTelemetryOptions)`

Configures OpenTelemetry tracing, metrics, and logging for the service collection.

**Parameters:**
- `services` - The service collection to add OpenTelemetry to
- `options` - The OpenTelemetry configuration options

**Returns:** The service collection for chaining

### `LoadOpenTelemetryOptions(IConfigurationManager)`

Loads and validates OpenTelemetry configuration options from the configuration section.

**Parameters:**
- `configuration` - The configuration manager to read from

**Returns:** A validated `OpenTelemetryOptions` instance

**Exceptions:**
- `InvalidOperationException` - Thrown when required configuration values (`ServiceName` or `ServiceVersion`) are missing or invalid

## Request Filtering

Request path filtering uses case-insensitive prefix matching. Any request path that starts with a path in the `RequestPathsToIgnore` list will be excluded from instrumentation.

For example, if `RequestPathsToIgnore` contains `"/healthz"`, the following paths will be ignored:
- `/healthz`
- `/healthz/ready`
- `/healthz/live`

But `/health` will still be instrumented.

Filtering applies to both:
- ASP.NET Core HTTP request instrumentation
- HttpClient outgoing request instrumentation

