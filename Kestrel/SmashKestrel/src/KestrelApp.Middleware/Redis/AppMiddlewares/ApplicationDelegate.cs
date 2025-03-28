namespace KestrelApp.Middleware.Redis.AppMiddlewares;

/// <summary>
/// 可以处理请求的委托
/// 中间件就是靠它来做请求传递的
/// 所以可以推断一个Func<ApplicationDelegate<TContext>, ApplicationDelegate<TContext>>就可以认为是一个应用层的中间件
/// </summary>
/// <typeparam name="TContext"></typeparam>
public delegate Task ApplicationDelegate<TContext>(TContext context);