using System.Net.Http.Headers;

namespace KestrelApp.Middleware.HttpProxy;

/// <summary>
/// 代理服务器对客户端身份认证
/// </summary>
sealed class HttpProxyAuthenticationHandler : IHttpProxyAuthenticationHandler
{
    public ValueTask<bool> AuthenticateAsync(AuthenticationHeaderValue? authentication)
    {
        return ValueTask.FromResult(true);
    }
}