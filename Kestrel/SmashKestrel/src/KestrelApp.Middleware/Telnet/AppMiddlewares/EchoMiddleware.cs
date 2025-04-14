using System.Text;
using KestrelApp.Common;

namespace KestrelApp.Middleware.Telnet.AppMiddlewares;

sealed class EchoMiddleware : IApplicationMiddleware<TelnetContext>
{
    public async Task InvokeAsync(ApplicationDelegate<TelnetContext> next, TelnetContext context)
    {
        await context.Response.WriteLineAsync($"刚输入的是{context.Request}么?", Encoding.UTF8);
    }
}