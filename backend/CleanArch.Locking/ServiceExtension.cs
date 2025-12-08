using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Locking;

public static class ServiceExtension
{
    public static IServiceCollection AddLocalLocking(this IServiceCollection services)
    {
        services.AddSingleton<ILocalLockService, LocalLockService>();
        return services;
    }
}
