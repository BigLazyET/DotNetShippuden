namespace KestrelApp.Middleware.Redis;

public class RedisContext
{
    public RedisClient Client { get; }
    
    public RedisRequest Request { get; }
    
    public RedisResponse Response { get; }

    public RedisContext(RedisClient client, RedisRequest request, RedisResponse response)
    {
        Client = client;
        Request = request;
        Response = response;
    }

    public override string ToString() => $"{Client} {Request}";
}