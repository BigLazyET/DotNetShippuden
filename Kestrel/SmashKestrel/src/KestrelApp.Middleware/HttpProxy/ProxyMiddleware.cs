using System.Buffers;
using System.IO.Pipelines;
using KestrelApp.Common;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

namespace KestrelApp.Middleware.HttpProxy;

sealed class ProxyMiddleware : IKestrelMiddleware
{
    private readonly HttpParser<HttpRequestHandler> _httpParser = new();
    
    public async Task InvokeAsync(ConnectionDelegate next, ConnectionContext context)
    {
        var input = context.Transport.Input;
        var requestHandler = new HttpRequestHandler();

        while (context.ConnectionClosed.IsCancellationRequested)
        {
            var result = await input.ReadAsync();
            if (result.IsCanceled)
                break;

            try
            {
                if (ParseRequest(result, requestHandler, out var consumed))
                {
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
    
    /// <summary>
    /// 解析http请求
    /// </summary>
    /// <param name="result"></param>
    /// <param name="consumed"></param>
    private bool ParseRequest(ReadResult result, HttpRequestHandler requestHandler, out int consumed)
    {
        var reader = new SequenceReader<byte>(result.Buffer);


        consumed = 0;
        return false;
    }
}