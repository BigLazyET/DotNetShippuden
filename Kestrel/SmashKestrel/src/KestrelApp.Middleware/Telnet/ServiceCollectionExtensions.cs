using KestrelApp.Middleware.Telnet.AppMiddlewares;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace KestrelApp.Middleware.Telnet;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTelnet(this IServiceCollection services)
    {
        services.TryAddSingleton<TelnetConnectionHandler>();
        return services;
    }
}