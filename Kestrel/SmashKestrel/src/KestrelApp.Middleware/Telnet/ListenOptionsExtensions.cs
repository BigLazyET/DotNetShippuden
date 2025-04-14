using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace KestrelApp.Middleware.Telnet;

public static class ListenOptionsExtensions
{
    public static ListenOptions UseTelnet(this ListenOptions options)
    {
        options.UseConnectionHandler<TelnetConnectionHandler>();
        return options;
    }
}