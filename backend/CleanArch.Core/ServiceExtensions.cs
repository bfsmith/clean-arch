using Microsoft.Extensions.DependencyInjection;
using CleanArch.Core.Services;

namespace CleanArch.Core;

/// <summary>
/// Extension methods for registering Core layer services with dependency injection.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Registers Core layer services with the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<IRandomNumberService, RandomNumberService>();
        return services;
    }
}
