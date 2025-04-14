using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using KestrelApp.Common;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

namespace KestrelApp.Middleware.HttpProxy;

/// <summary>
/// kestrel层面的代理中间件
/// 其目的是解析请求来填充自定义的proxy feature
/// </summary>
sealed class ProxyMiddleware : IKestrelMiddleware
{
    private static byte[] Http400 = Encoding.ASCII.GetBytes("HTTP/1.1 400 Bad Request\r\n\r\n");
    private static byte[] Http407 = Encoding.ASCII.GetBytes("HTTP/1.1 407 Proxy Authentication Required\r\n\r\n");
    
    private readonly HttpParser<HttpRequestHandler> _httpParser = new();
    private readonly IHttpProxyAuthenticationHandler _authenticationHandler;

    public ProxyMiddleware(IHttpProxyAuthenticationHandler authenticationHandler)
    {
        _authenticationHandler = authenticationHandler;
    }
    
    public async Task InvokeAsync(ConnectionDelegate next, ConnectionContext context)
    {
        var input = context.Transport.Input;
        var output = context.Transport.Output;
        var requestHandler = new HttpRequestHandler();

        while (context.ConnectionClosed.IsCancellationRequested == false)
        {
            var result = await input.ReadAsync();
            if (result.IsCanceled)
                break;

            try
            {
                if (ParseRequest(result, requestHandler, out var consumed))
                {
                    if (requestHandler.ProxyProtocol == ProxyProtocol.TunnelProxy)
                    {
                        input.AdvanceTo(consumed);
                    }
                    else
                    {
                        input.AdvanceTo(result.Buffer.Start);
                    }

                    if (await _authenticationHandler.AuthenticateAsync(requestHandler.ProxyAuthorization))
                    {
                        context.Features.Set<IProxyFeature>(requestHandler);
                        await next(context);
                    }
                    else
                    {
                        await output.WriteAsync(Http407);
                    }
                }
                else
                {
                    input.AdvanceTo(result.Buffer.Start, result.Buffer.End);
                }
                
                if (result.IsCompleted)
                    break;
            }
            catch (Exception e)
            {
                await output.WriteAsync(Http400);
                throw;
            }
        }
    }
    
    /// <summary>
    /// 解析http请求
    /// </summary>
    /// <param name="result"></param>
    /// <param name="requestHandler"></param>
    /// <param name="consumed"></param>
    /// <returns></returns>
    private bool ParseRequest(ReadResult result, HttpRequestHandler requestHandler, out SequencePosition consumed)
    {
        var reader = new SequenceReader<byte>(result.Buffer);

        if (_httpParser.ParseRequestLine(requestHandler, ref reader) &&
            _httpParser.ParseHeaders(requestHandler, ref reader))
        {
            consumed = reader.Position;
            return true;
        }

        consumed = default;
        return false;
    }
}