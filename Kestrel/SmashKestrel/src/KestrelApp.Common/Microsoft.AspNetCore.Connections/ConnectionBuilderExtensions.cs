using KestrelApp.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Connections;

public static class ConnectionBuilderExtensions
{
    public static IConnectionBuilder Use<T>(this IConnectionBuilder builder) where T : IKestrelMiddleware
    {
        var middleware = ActivatorUtilities.CreateInstance<T>(builder.ApplicationServices);
        return builder.Use(middleware);
    }

    public static IConnectionBuilder Use(this IConnectionBuilder builder, IKestrelMiddleware middleware)
    {
        builder.Use(next => context => middleware.InvokeAsync(next, context));
        return builder;
    }
}