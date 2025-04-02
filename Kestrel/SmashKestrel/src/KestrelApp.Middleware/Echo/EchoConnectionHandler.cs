using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using System.Text;
using KestrelApp.Common.System.Buffers;
using Microsoft.AspNetCore.Connections;

namespace KestrelApp.Middleware.Echo;

sealed class EchoConnectionHandler : ConnectionHandler
{
    public override async Task OnConnectedAsync(ConnectionContext connection)
    {
        var input = connection.Transport.Input;
        var output = connection.Transport.Output;
        
        while (connection.ConnectionClosed.IsCancellationRequested == false)
        {
            var result = await input.ReadAsync();
            if (result.IsCanceled)
                break;

            if (TryEcho(result, out var echo, out var consumed))
            {
                using (echo)
                {
                    // var text = Encoding.UTF8.GetString(echo.AsSpan());
                    var text = Encoding.UTF8.GetString(echo.Array, 0, echo.Length);
                    output.WriteBigEndian((ushort)echo.Length);
                    output.Write(echo.Array.AsSpan().Slice(0, echo.Length));
                    await output.FlushAsync();
                }
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

    private bool TryEcho(ReadResult readResult, [MaybeNullWhen(false)]out IArrayOwner<byte> echo, out SequencePosition consumed)
    {
        var reader = new SequenceReader<byte>(readResult.Buffer);
        if (reader.TryReadBigEndian(out short length))
        {
            if (reader.Remaining >= length)
            {
                echo = ArrayPool<byte>.Shared.RentArrayOwner(length);
                reader.UnreadSequence.Slice(0, length).CopyTo(echo.Array);
                reader.Advance(length);
                
                consumed = reader.Position;
                return true;
            }
        }

        echo = null;
        consumed = readResult.Buffer.Start;
        return false;
    }
}