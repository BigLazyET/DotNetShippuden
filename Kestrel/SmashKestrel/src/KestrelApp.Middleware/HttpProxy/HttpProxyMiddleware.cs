using System.Net;
using Microsoft.AspNetCore.Http;
using Yarp.ReverseProxy.Forwarder;

namespace KestrelApp.Middleware.HttpProxy;

/// <summary>
/// 应用层面的Http代理中间件
/// </summary>
public sealed class HttpProxyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHttpForwarder _httpForwarder;
    private readonly HttpMessageInvoker _httpClient = new HttpClient(CreateSocketsHttpHandler());

    public HttpProxyMiddleware(RequestDelegate next, IHttpForwarder httpForwarder)
    {
        _next = next;
        _httpForwarder = httpForwarder;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var feature = context.Features.Get<IProxyFeature>();
        if (feature == null)
            await _next(context);
        else if (feature.ProxyProtocol == ProxyProtocol.None)
            await context.Response.WriteAsJsonAsync(new { Err = "请用http代理协议访问" });
        else
        {
            var scheme = context.Request.Scheme;
            var destinationPrefix = $"{scheme}://{feature.ProxyHost}";
            await _httpForwarder.SendAsync(context, destinationPrefix, _httpClient, ForwarderRequestConfig.Empty,
                HttpTransformer.Empty);
        }
    }
    
    private static SocketsHttpHandler CreateSocketsHttpHandler()
    {
        return new SocketsHttpHandler
        {
            Proxy = null,
            UseProxy = false,
            UseCookies = false,
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.None,
        };
    }
}