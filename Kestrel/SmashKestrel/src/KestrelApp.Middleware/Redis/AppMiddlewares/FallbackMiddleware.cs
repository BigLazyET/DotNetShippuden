using KestrelApp.Common;

namespace KestrelApp.Middleware.Redis;

sealed class FallbackMiddleware : IRedisMiddleware
{
    public async Task InvokeAsync(ApplicationDelegate<RedisContext> next, RedisContext context)
    {
        await context.Response.WriteAsync(ResponseContent.Err);
    }
}