using KestrelApp.Common;

namespace KestrelApp.Middleware.Redis;

sealed class CmdMiddleware : IRedisMiddleware
{
    private readonly IDictionary<RedisCmd, IRedisCmdHandler> _cmdHandlers;

    public CmdMiddleware(IEnumerable<IRedisCmdHandler> cmdHandlers)
    {
        _cmdHandlers = cmdHandlers.ToDictionary(h => h.Cmd, h => h);
    }
    
    public async Task InvokeAsync(ApplicationDelegate<RedisContext> next, RedisContext context)
    {
        if (_cmdHandlers.TryGetValue(context.Request.Cmd, out var handler))
        {
            await handler.HandleAsync(context);
        }
        else
        {
            await next(context);
        }
    }
}