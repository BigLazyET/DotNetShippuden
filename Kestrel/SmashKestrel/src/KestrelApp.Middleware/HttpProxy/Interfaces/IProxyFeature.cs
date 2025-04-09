using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace KestrelApp.Middleware.HttpProxy;

/// <summary>
/// 代理Feature
/// </summary>
public interface IProxyFeature
{
    /// <summary>
    /// 代理的主机，而不是代理服务器本身
    /// </summary>
    public HostString ProxyHost { get; }
    
    /// <summary>
    /// 代理协议
    /// </summary>
    public ProxyProtocol ProxyProtocol { get; }
    
    /// <summary>
    /// 代理认证信息
    /// </summary>
    public AuthenticationHeaderValue? Authentication { get; }
}