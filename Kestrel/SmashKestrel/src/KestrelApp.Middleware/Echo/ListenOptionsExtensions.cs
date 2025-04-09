using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace KestrelApp.Middleware.Echo;

public static class ListenOptionsExtensions
{
    public static ListenOptions UseEcho(this ListenOptions options)
    {
        options.UseConnectionHandler<EchoConnectionHandler>();
        return options;
    }
}