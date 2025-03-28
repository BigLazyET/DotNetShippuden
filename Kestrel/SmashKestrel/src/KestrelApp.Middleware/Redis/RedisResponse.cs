using System.IO.Pipelines;

namespace KestrelApp.Middleware.Redis;

public class RedisResponse
{
    private readonly PipeWriter _writer;

    public RedisResponse(PipeWriter writer)
    {
        _writer = writer;
    }
}