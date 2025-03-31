using System.Text;
using Microsoft.Extensions.Options;

namespace KestrelApp.Middleware.Redis;

sealed class AuthCmdHandler : IRedisCmdHandler
{
    private readonly IOptionsMonitor<RedisOptions> _options;
    
    public RedisCmd Cmd => RedisCmd.Auth;

    public AuthCmdHandler(IOptionsMonitor<RedisOptions> options)
    {
        _options = options;
    }
    
    public async ValueTask HandleAsync(RedisContext context)
    {
        var auth = _options.CurrentValue.Auth;
        if (string.IsNullOrWhiteSpace(auth))
            context.Client.IsAuthed = true;
        else if (context.Request.ArgumentCount == 0)
            context.Client.IsAuthed = false;
        else
        {
            var password = context.Request.Argument(0).Value;
            context.Client.IsAuthed = password.Span.SequenceEqual(Encoding.UTF8.GetBytes(auth));
        }

        if (context.Client.IsAuthed == true)
            await context.Response.WriteAsync(ResponseContent.Ok);
        else
            await context.Response.WriteAsync(ResponseContent.Err);
    }
}