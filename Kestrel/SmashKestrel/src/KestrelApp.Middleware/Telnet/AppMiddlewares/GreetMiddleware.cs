using System.Text;
using KestrelApp.Common;

namespace KestrelApp.Middleware.Telnet.AppMiddlewares;

sealed class GreetMiddleware : IApplicationMiddleware<TelnetContext>
{
    private const string Greeting = "Hello";
    
    public async Task InvokeAsync(ApplicationDelegate<TelnetContext> next, TelnetContext context)
    {
        if (context.Request.Equals(Greeting))
        {
            context.Response.WriteLine("Hello, nice to meet u", Encoding.UTF8);
            await context.Response.FlushAsync();
        }
        else
        {
            await next(context);
        }
    }
}