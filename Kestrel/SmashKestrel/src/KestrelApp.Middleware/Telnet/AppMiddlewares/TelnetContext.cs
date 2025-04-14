using KestrelApp.Common;
using KestrelApp.Middleware.Telnet.Responses;
using Microsoft.AspNetCore.Connections;

namespace KestrelApp.Middleware.Telnet.AppMiddlewares;

sealed class TelnetContext : ApplicationContext
{
    public string Request { get; }
    
    public ConnectionContext Context { get; }
    
    public TelnetResponse Response { get; }
    
    public TelnetContext(string request, TelnetResponse response, ConnectionContext context) : base(context.Features)
    {
        Request = request;
        Response = response;
        Context = context;
    }

    public void Close()
    {
        Context.Abort();
    }
}