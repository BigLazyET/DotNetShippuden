using System.Net.Sockets;
using System.Text;
using KestrelApp.Common;
using Microsoft.AspNetCore.Connections;
using System.IO.Pipelines;

namespace KestrelApp.Middleware.HttpProxy;

/// <summary>
/// Kestrel层面的隧道代理中间件
/// 其目的是创建代理服务器与目标服务器的tcp连接
/// </summary>
sealed class TunnelProxyMiddleware : IKestrelMiddleware
{
    private readonly byte[] Http200 = Encoding.ASCII.GetBytes("HTTP/1.1 200 Connection Established\r\n\r\n");
    private readonly byte[] Http502 = Encoding.ASCII.GetBytes("HTTP/1.1 502 Bad Gateway\r\n\r\n");
    
    public async Task InvokeAsync(ConnectionDelegate next, ConnectionContext context)
    {
        var feature = context.Features.Get<IProxyFeature>();
        if (feature is { ProxyProtocol: ProxyProtocol.TunnelProxy })
        {
            await ProcessTunnelRequest(context, feature);
        }
        else
        {
            await next(context);
        }
    }

    private async ValueTask ProcessTunnelRequest(ConnectionContext context, IProxyFeature feature)
    {
        var output = context.Transport.Output;

        var port = feature.ProxyHost.Port;
        if (port.HasValue == false)
        {
            await output.WriteAsync(Http502);
            return;
        }

        using var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        try
        {
            var host = feature.ProxyHost.Host;
            await socket.ConnectAsync(host, port.Value, context.ConnectionClosed);
            await output.WriteAsync(Http200);
        }
        catch (Exception ex)
        {
            await output.WriteAsync(Http502);
            return;
        }

        var stream = new NetworkStream(socket, ownsSocket: false);
        var task1 = stream.CopyToAsync(output);
        var task2 = context.Transport.Input.CopyToAsync(stream);
        await Task.WhenAny(task1, task2);
    }
}