using System.Text;
using KestrelApp.Common;

namespace KestrelApp.Middleware.Telnet.AppMiddlewares;

sealed class EmptyMiddleware : IApplicationMiddleware<TelnetContext>
{
    public async Task InvokeAsync(ApplicationDelegate<TelnetContext> next, TelnetContext context)
    {
        if (string.IsNullOrWhiteSpace(context.Request))
        {
            await context.Response.WriteLineAsync("请输入...", Encoding.UTF8);
        }
        else
        {
            await next(context);
        }
    }
}