using System.IO.Pipelines;
using System.Text;
using KestrelApp.Common.System.Buffers;

namespace KestrelApp.Middleware.Telnet.Responses;

public class TelnetResponse
{
    private readonly PipeWriter _pipeWriter;
    
    public TelnetResponse(PipeWriter pipeWriter)
    {
        _pipeWriter = pipeWriter;
    }

    public TelnetResponse WriteLine(ReadOnlySpan<char> text, Encoding? encoding)
    {
        _pipeWriter.Write(text, encoding ?? Encoding.UTF8);
        _pipeWriter.WriteCrlf();
        return this;
    }

    public ValueTask<FlushResult> WriteLineAsync(ReadOnlySpan<char> text, Encoding? encoding)
    {
        WriteLine(text, encoding);
        return FlushAsync();
    }

    public ValueTask<FlushResult> FlushAsync()
    {
        return _pipeWriter.FlushAsync();
    }
    
}