using Microsoft.AspNetCore.Connections;

namespace KestrelApp.Middleware.Redis;

/// <summary>
/// Redis连接handler
/// 核心思想，便于理解
/// 1. 注意：在Kestrel中，末级的中间件是一个没有next的特殊中间件，基表现出来就是一个ConnectionHandler的行为，我们这边继承的就是这个
/// 2. 我们接管到这个连接后，接下来就交由我们自己处理，我们可以创建自己的中间件以及RedisContext来做相关的一些处理，包括认证，命令处理，报错返回等等
/// 3. 可以对标这么理解：我们自定义的中间件和RedisContext其实概念等同于跟
/// [在asp.netcore的Startup里，我们使用app.UseXXX的扩展方法来应用各种中间件，比如UseRouting、UseStaticFiles等等，
/// 它本质上还是调用了IApplicationBuilder.Use(Func<RequestDelegate, RequestDelegate> middleware)，也就说Func<RequestDelegate, RequestDelegate>就是一个中间件]
///
/// 这里面有两个层面的中间件：Kestrel层级的中间件(可以理解为传输层中间件)和应用层级的中间件
/// </summary>
public class RedisConnectionHandler : ConnectionHandler
{
    /// <summary>
    /// 处理Redis连接
    /// </summary>
    /// <param name="connection">ConnectionContext是kestrel的一个Tcp连接抽象，其核心属性是Transport，表示双工传输层的操作对象，另外提供Abort()方法用于服务端主动关闭连接</param>
    public override async Task OnConnectedAsync(ConnectionContext connection)
    {
        try
        {

        }
        catch (Exception e)
        {

        }
        finally
        {
            await connection.DisposeAsync();
        }
    }

    /// <summary>
    /// 处理Redis请求
    /// </summary>
    /// <param name="connection"></param>
    private async Task HandleRequestAsync(ConnectionContext connection)
    {
        var duplexPipe = connection.Transport;
        var input = duplexPipe.Input;
        var client = new RedisClient(connection);
        var response = new RedisResponse(duplexPipe.Output);

        while (connection.ConnectionClosed.IsCancellationRequested == false)
        {
            var result = await input.ReadAsync();
            if (result.IsCanceled)
                break;

            var requests = RedisRequest.Parse(result.Buffer, out var consumed);
            if (requests.Count > 0)
            {
                foreach (var request in requests)
                {
                    var context = new RedisContext(client, request, response);
                    // 传到应用层的中间件去做进一步处理
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
}