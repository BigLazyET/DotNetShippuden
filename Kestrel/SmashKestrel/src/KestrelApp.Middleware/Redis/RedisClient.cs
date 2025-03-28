using System.Net;
using Microsoft.AspNetCore.Connections;

namespace KestrelApp.Middleware.Redis;

public class RedisClient
{
    private readonly ConnectionContext _connectionContext;
    
    public bool? IsAuth { get; set; }

    public EndPoint? RemoteEndPoint => _connectionContext.RemoteEndPoint;

    public RedisClient(ConnectionContext connectionContext)
    {
        _connectionContext = connectionContext;
    }

    public void Close() => _connectionContext.Abort();

    public override string? ToString() => RemoteEndPoint?.ToString();
}