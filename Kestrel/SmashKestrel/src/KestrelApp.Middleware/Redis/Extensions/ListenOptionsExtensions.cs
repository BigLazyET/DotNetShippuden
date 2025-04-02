using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace KestrelApp.Middleware.Redis;

public static partial class ListenOptionsExtensions
{
    public static ListenOptions UseRedis(this ListenOptions options)
    {
        options.UseConnectionHandler<RedisConnectionHandler>();
        return options;
    }
}