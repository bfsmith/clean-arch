using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CleanArch.Logging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCleanLogging(
        this IServiceCollection services,
        IConfiguration? configuration)
    {
        // Create Serilog logger
        var loggerConfiguration = SerilogConfiguration.CreateLoggerConfiguration();
        
        if (configuration != null)
        {
            loggerConfiguration = loggerConfiguration.ReadFrom.Configuration(configuration);
        }

        Log.Logger = loggerConfiguration.CreateLogger();

        // Replace the default logging with Serilog
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog(Log.Logger, dispose: true);
        });

        return services;
    }
}

