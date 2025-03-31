namespace KestrelApp.Middleware.Redis;

sealed class PingCmdHandler : IRedisCmdHandler
{
    public RedisCmd Cmd => RedisCmd.Ping;
    
    public async ValueTask HandleAsync(RedisContext context)
    {
        await context.Response.WriteAsync(ResponseContent.Pong);
    }
}