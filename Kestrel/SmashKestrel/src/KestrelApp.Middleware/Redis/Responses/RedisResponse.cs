using System.IO.Pipelines;

namespace KestrelApp.Middleware.Redis;

sealed class RedisResponse
{
    private readonly PipeWriter _writer;

    public RedisResponse(PipeWriter writer)
    {
        _writer = writer;
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