using KestrelApp.Common;
using Microsoft.Extensions.Options;

namespace KestrelApp.Middleware.Redis;

sealed class AuthMiddleware : IRedisMiddleware
{
    private readonly IOptionsMonitor<RedisOptions> _options;
    
    public AuthMiddleware(IOptionsMonitor<RedisOptions> options)
    {
        _options = options;
    }
    
    public async Task InvokeAsync(ApplicationDelegate<RedisContext> next, RedisContext context)
    {
        if (context.Client.IsAuthed == false)
            await context.Response.WriteAsync(ResponseContent.Err);
        else if (context.Client.IsAuthed == true)
            await next(context);
        // 当 Redis 启用密码时，客户端在建立连接后需要首先执行一次 `AUTH <password>` 命令进行身份验证。成功验证后，该连接会被标记为已验证，后续的命令无需再重复 `AUTH`。
        // 因此，后续每个命令并不需要自动带上 `AUTH <password>` 前缀。
        // 每个新的连接都需要重新进行身份验证，但对于已经验证的连接，后续的命令可以直接执行。
        else if (context.Request.Cmd != RedisCmd.Auth)
        {
            if (string.IsNullOrWhiteSpace(_options.CurrentValue.Auth))
            {
                // Redis没有启用密码保护
                context.Client.IsAuthed = true;
                await next(context);
            }
            else
            {
                await context.Response.WriteAsync(ResponseContent.Err);
            }
        }
        else
        {
            // Auth命令应该首先执行，从而为整个连接定性：标记为是否已验证 - 这里的连接可以认为是RedisClient
            await next(context);
        }
    }
}