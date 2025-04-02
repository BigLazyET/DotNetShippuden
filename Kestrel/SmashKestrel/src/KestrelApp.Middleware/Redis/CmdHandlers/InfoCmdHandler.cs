namespace KestrelApp.Middleware.Redis;

sealed class InfoCmdHandler : IRedisCmdHandler
{
    public RedisCmd Cmd => RedisCmd.Info;

    public async ValueTask HandleAsync(RedisContext context)
    {
        // 编译时确定确定值并嵌入代码，避免运行时再分配，初始化以及垃圾回收负担
        const string info = "Redis Version 6.6.6";

        await context.Response.Write('$').Write(info.Length.ToString()).WriteLine()
            .Write(info).WriteLine()
            .FlushAsync();
    }
}