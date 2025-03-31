namespace KestrelApp.Middleware.Redis;

sealed class EchoCmdHandler : IRedisCmdHandler
{
    public RedisCmd Cmd => RedisCmd.Echo;
    
    public ValueTask HandleAsync(RedisContext context)
    {
        throw new NotImplementedException();
    }
}