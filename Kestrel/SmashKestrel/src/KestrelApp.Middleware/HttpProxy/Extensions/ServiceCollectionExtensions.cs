using Microsoft.Extensions.DependencyInjection;

namespace KestrelApp.Middleware.HttpProxy.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHttpProxy(this IServiceCollection services)
    {
        services.AddSingleton<IHttpProxyAuthenticationHandler, HttpProxyAuthenticationHandler>();
        services.AddSingleton<ProxyMiddleware>().AddSingleton<TunnelProxyMiddleware>().AddHttpForwarder();
        
        return services;
    }
}