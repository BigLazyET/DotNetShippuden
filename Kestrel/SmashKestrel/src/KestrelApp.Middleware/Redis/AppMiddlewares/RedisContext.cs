using KestrelApp.Common;
using Microsoft.AspNetCore.Http.Features;

namespace KestrelApp.Middleware.Redis;

sealed class RedisContext : ApplicationContext
{
    public RedisClient Client { get; }
    
    public RedisRequest Request { get; }
    
    public RedisResponse Response { get; }

    public RedisContext(RedisClient client, RedisRequest request, RedisResponse response, IFeatureCollection features) : base(features)
    {
        Client = client;
        Request = request;
        Response = response;
    }

    public override string ToString() => $"{Client} {Request}";
}