namespace KestrelApp.Middleware.Redis;

sealed class SelectCmdHandler : IRedisCmdHandler
{
    public RedisCmd Cmd => RedisCmd.Select;
    
    public async ValueTask HandleAsync(RedisContext context)
    {
        await context.Response.WriteAsync(ResponseContent.Ok);
    }
}