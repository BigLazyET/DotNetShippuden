using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Connections;

namespace KestrelApp.Middleware.HttpProxy.Extensions;

public static class ListenOptionsExtensions
{
    public static ListenOptions UseHttpProxy(this ListenOptions options)
    {
        options.Use<ProxyMiddleware>();
        options.Use<TunnelProxyMiddleware>();
        
        return options;
    }
}