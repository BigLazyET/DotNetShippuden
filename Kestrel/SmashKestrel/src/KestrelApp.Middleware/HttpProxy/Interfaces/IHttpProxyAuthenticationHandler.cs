using System.Net.Http.Headers;

namespace KestrelApp.Middleware.HttpProxy;

/// <summary>
/// Http代理身份验证hanlder
/// </summary>
public interface IHttpProxyAuthenticationHandler
{
    /// <summary>
    /// 身份认证
    /// </summary>
    /// <param name="authentication"></param>
    /// <returns></returns>
    ValueTask<bool> AuthenticateAsync(AuthenticationHeaderValue? authentication);
}