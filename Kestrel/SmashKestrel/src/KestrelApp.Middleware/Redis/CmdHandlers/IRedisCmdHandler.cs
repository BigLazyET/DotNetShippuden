namespace KestrelApp.Middleware.Redis;

/// <summary>
/// RedisRequest的处理者
/// </summary>
interface IRedisCmdHandler
{
    /// <summary>
    /// 当前命令处理器对应的命令类型
    /// </summary>
    public RedisCmd Cmd { get; }

    /// <summary>
    /// 处理命令
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    ValueTask HandleAsync(RedisContext context);
}