namespace KestrelApp.Middleware.Redis;

sealed class QuitCmdHandler : IRedisCmdHandler
{
    public RedisCmd Cmd => RedisCmd.Quit;
    
    public ValueTask HandleAsync(RedisContext context)
    {
        context.Client.Close();
        return ValueTask.CompletedTask;
    }
}