using Microsoft.Extensions.DependencyInjection;

namespace KestrelApp.Middleware.Redis.AppMiddlewares;

public class ApplicationBuilder<TContext>
{
    private readonly ApplicationDelegate<TContext> fallbackHandler;
    
    public IServiceProvider ApplicationServices { get; }

    /// <summary>
    /// 中间件集合
    /// </summary>
    private readonly List<Func<ApplicationDelegate<TContext>, ApplicationDelegate<TContext>>> middlewares = new();

    public ApplicationBuilder(IServiceProvider serviceProvider)
    {
        ApplicationServices = serviceProvider;
    }

    public ApplicationDelegate<TContext> Build()
    {
        var handler = fallbackHandler;
        // 中间件需要从前往后执行，所以嵌套需要从后往前嵌套
        for (int i = middlewares.Count - 1; i > 0; i--)
        {
            handler = middlewares[i](fallbackHandler);
        }

        return handler;
    }

    public ApplicationBuilder<TContext> Use<TMiddleware>() where TMiddleware : IApplicationMiddleware<TContext>
    {
        var middleware = ActivatorUtilities.GetServiceOrCreateInstance<TMiddleware>(ApplicationServices);
        return Use(middleware);
    }

    public ApplicationBuilder<TContext> Use<TMiddleware>(TMiddleware middleware) where TMiddleware : IApplicationMiddleware<TContext>
    {
        return Use(middleware.InvokeAsync);
    }
    
    public ApplicationBuilder<TContext> Use(Func<ApplicationDelegate<TContext>, TContext, Task> middleware)
    {
        return Use(next => context => middleware(next, context));
    }

    public ApplicationBuilder<TContext> Use(
        Func<ApplicationDelegate<TContext>, ApplicationDelegate<TContext>> middleware)
    {
        middlewares.Add(middleware);
        return this;
    }
}