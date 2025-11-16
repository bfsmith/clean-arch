using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace CleanArch.Logging;

public static class SerilogConfiguration
{
    public static LoggerConfiguration CreateLoggerConfiguration()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .WriteTo.Console(new JsonFormatter());
    }
}

