namespace KestrelApp.Middleware.HttpProxy;

/// <summary>
/// 代理协议
/// Http代理分为: 普通Http代理和隧道代理
/// 普通 HTTP 代理：处理未加密的流量，能够查看和修改数据。功能：处理和转发 HTTP 请求和响应。
/// 隧道代理：用于加密流量的转发，不查看数据内容。功能：通过 CONNECT 方法建立一个端到端的 TCP 隧道。
/// </summary>
public enum ProxyProtocol
{
    /// <summary>
    /// 无代理
    /// </summary>
    None,
    /// <summary>
    /// Http代理
    /// </summary>
    HttpProxy,
    /// <summary>
    /// 隧道代理
    /// </summary>
    TunnelProxy
}