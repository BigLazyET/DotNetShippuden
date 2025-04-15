using Microsoft.Extensions.DependencyInjection;

namespace KestrelApp.Common;

public class ApplicationBuilder<TContext>
{
    private readonly ApplicationDelegate<TContext> _fallbackHandler;
    
    public IServiceProvider ApplicationServices { get; }

    /// <summary>
    /// 中间件集合
    /// </summary>
    private readonly List<Func<ApplicationDelegate<TContext>, ApplicationDelegate<TContext>>> middlewares = new();

    public ApplicationBuilder(IServiceProvider serviceProvider) : this(serviceProvider, context => Task.CompletedTask)
    {
        
    }

    public ApplicationBuilder(IServiceProvider serviceProvider, ApplicationDelegate<TContext> fallbackHandler)
    {
        ApplicationServices = serviceProvider;
        _fallbackHandler = fallbackHandler;
    }

    public ApplicationDelegate<TContext> Build()
    {
        var handler = _fallbackHandler;
        // 中间件需要从前往后执行，所以嵌套需要从后往前嵌套
        for (var i = middlewares.Count - 1; i >= 0; i--)
        {
            handler = middlewares[i](handler);
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