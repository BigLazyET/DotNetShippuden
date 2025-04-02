using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using KestrelApp.Common.System.Buffers;

namespace KestrelApp.Middleware.Redis;

sealed class RedisResponse
{
    private readonly PipeWriter _writer;

    public RedisResponse(PipeWriter writer)
    {
        _writer = writer;
    }

    public RedisResponse Write(char value)
    {
        var span = _writer.GetSpan(1);
        span[0] = (byte)value;
        _writer.Advance(1);
        return this;
    }

    public RedisResponse Write(ReadOnlySpan<char> value)
    {
        _writer.Write(value, Encoding.UTF8);
        return this;
    }

    public RedisResponse Write(ReadOnlyMemory<byte> value)
    {
        _writer.Write(value.Span);
        return this;
    }

    public RedisResponse WriteLine()
    {
        // _writer.Write("\r\n"u8);

        var span = _writer.GetSpan(2);
        span[0] = (byte)'\r';
        span[1] = (byte)'\n';
        _writer.Advance(2);
        return this;
    }

    public ValueTask<FlushResult> FlushAsync()
    {
        return _writer.FlushAsync();
    }
    
    public ValueTask<FlushResult> WriteAsync(ResponseContent content)
    {
        return _writer.WriteAsync(content.ToMemory());
    }
}