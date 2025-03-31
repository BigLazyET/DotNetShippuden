namespace KestrelApp.Middleware.Redis;

/// <summary>
/// Redis选项
/// </summary>
public record RedisOptions
{
    /// <summary>
    /// 密钥
    /// </summary>
    public string? Auth { get; set; }
}