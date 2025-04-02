namespace KestrelApp.Middleware.Redis;

sealed class EchoCmdHandler : IRedisCmdHandler
{
    public RedisCmd Cmd => RedisCmd.Echo;

    public async ValueTask HandleAsync(RedisContext context)
    {
        var echo = context.Request.ArgumentCount > 0
            ? context.Request.Argument(0)
            : new RedisValue(ReadOnlyMemory<byte>.Empty);

        // 协议格式
        // $2
        // xx
        await context.Response.Write('$').Write(echo.Value.Length.ToString()).WriteLine()
            .Write(echo.Value).WriteLine()
            .FlushAsync();
    }
}