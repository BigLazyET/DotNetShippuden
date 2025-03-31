using KestrelApp.Common;

namespace KestrelApp.Middleware.Redis;

/// <summary>
/// Redis 应用中间件
/// </summary>
interface IRedisMiddleware : IApplicationMiddleware<RedisContext>
{
    
}