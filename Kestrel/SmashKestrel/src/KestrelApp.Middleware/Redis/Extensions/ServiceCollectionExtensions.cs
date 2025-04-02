using Microsoft.Extensions.DependencyInjection;

namespace KestrelApp.Middleware.Redis;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRedis(this IServiceCollection services)
    {
        services.AddSingleton<IRedisCmdHandler, AuthCmdHandler>();
        services.AddSingleton<IRedisCmdHandler, EchoCmdHandler>();
        services.AddSingleton<IRedisCmdHandler, InfoCmdHandler>();
        services.AddSingleton<IRedisCmdHandler, PingCmdHandler>();
        services.AddSingleton<IRedisCmdHandler, QuitCmdHandler>();
        services.AddSingleton<IRedisCmdHandler, SelectCmdHandler>();
        
        return services;
    }
}