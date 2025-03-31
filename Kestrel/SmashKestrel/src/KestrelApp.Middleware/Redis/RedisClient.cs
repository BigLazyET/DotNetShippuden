using System.Net;
using Microsoft.AspNetCore.Connections;

namespace KestrelApp.Middleware.Redis;

public class RedisClient
{
    private readonly ConnectionContext _connectionContext;
    
    /// <summary>
    /// 是否已经授权
    /// 其默认值为null
    /// </summary>
    public bool? IsAuthed { get; set; }

    public EndPoint? RemoteEndPoint => _connectionContext.RemoteEndPoint;

    public RedisClient(ConnectionContext connectionContext)
    {
        _connectionContext = connectionContext;
    }

    public void Close() => _connectionContext.Abort();

    public override string? ToString() => RemoteEndPoint?.ToString();
}