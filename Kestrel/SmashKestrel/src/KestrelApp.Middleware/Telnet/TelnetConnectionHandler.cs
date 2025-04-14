using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Text;
using KestrelApp.Common;
using KestrelApp.Middleware.Telnet.AppMiddlewares;
using KestrelApp.Middleware.Telnet.Responses;
using Microsoft.AspNetCore.Connections;

namespace KestrelApp.Middleware.Telnet;

public sealed class TelnetConnectionHandler : ConnectionHandler
{
    private static byte[] Delimiter = Encoding.ASCII.GetBytes("\r\n");

    private ApplicationDelegate<TelnetContext> _application;

    public TelnetConnectionHandler(IServiceProvider serviceProvider)
    {
        _application = new ApplicationBuilder<TelnetContext>(serviceProvider)
            .Use<EmptyMiddleware>()
            .Use<GreetMiddleware>()
            .Use<EchoMiddleware>()
            .Build();
    }
    
    public override async Task OnConnectedAsync(ConnectionContext connection)
    {
        var input = connection.Transport.Input;
        var output = connection.Transport.Output;

        var response = new TelnetResponse(output);
        response.WriteLine($"欢迎", Encoding.UTF8);
        response.WriteLine($"现在是{DateTime.Now}", Encoding.UTF8);
        await response.FlushAsync();

        while (connection.ConnectionClosed.IsCancellationRequested == false)
        {
            var result = await input.ReadAsync();
            if (result.IsCanceled)
                break;
            if (TryReadRequest(result, out var request, out var consumed))
            {
                // 传到应用层的中间件去做进一步处理
                var context = new TelnetContext(request, response, connection);
                await _application.Invoke(context);
                input.AdvanceTo(consumed);
            }
            else
            {
                input.AdvanceTo(result.Buffer.Start, result.Buffer.End);
            }

            if (result.IsCompleted)
                break;
        }
    }

    private bool TryReadRequest(ReadResult result, out string request, out SequencePosition consumed)
    {
        var reader = new SequenceReader<byte>(result.Buffer);
        if (reader.TryReadTo(out ReadOnlySpan<byte> span, Delimiter))
        {
            request = Encoding.UTF8.GetString(span);
            Debug.WriteLine($"telnet get: {request}");
            consumed = reader.Position;
            return true;
        }

        request = string.Empty;
        consumed = result.Buffer.Start;
        return false;
    }
}