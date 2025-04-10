using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace KestrelApp.Middleware.HttpProxy;

// 一个完整的 HTTP 请求信息通常包括以下部分：
// 
// ### 请求行
// - **方法**：HTTP 方法（如 GET、POST）。
// - **URL**：请求的目标路径或资源。
// - **HTTP 版本**：如 HTTP/1.1。
// 
// 示例：
// ```
// GET /index.html HTTP/1.1
// ```
// 
// ### 请求头部
// - **键值对**的形式，提供请求的元数据。
// - 常见头部：
//   - `Host`: 请求的目标主机。
//   - `User-Agent`: 发起请求的客户端信息。
//   - `Accept`: 客户端可接收的媒体类型。
//   - `Content-Type`: 请求主体的数据类型（如 POST 请求）。
//   - `Content-Length`: 请求主体的长度。
// 
// 示例：
// ```
// Host: www.example.com
// User-Agent: Mozilla/5.0
// Accept: text/html
// ```
// 
// ### 空行
// - 用于分隔请求头部和请求主体。
// 
// ### 请求主体（可选）
// - 包含实际要发送的数据，如表单数据或文件上传内容。
// - 仅在需要时存在（如 POST 请求）。
// 
// 完整示例：
// ```
// POST /submit-form HTTP/1.1
// Host: www.example.com
// User-Agent: Mozilla/5.0
// Content-Type: application/x-www-form-urlencoded
// Content-Length: 27
// 
// field1=value1&field2=value2
// ```
// 
// 这个示例展示了一个 POST 请求，包含请求行、头部、空行和请求主体。

/// <summary>
/// 代理请求Handler
/// </summary>
sealed class HttpRequestHandler : IHttpRequestLineHandler, IHttpHeadersHandler, IProxyFeature
{
    private HttpMethod _method;
    
    public HostString ProxyHost { get; private set; }


    public ProxyProtocol ProxyProtocol
    {
        get
        {
            if (ProxyHost.HasValue == false)
                return ProxyProtocol.None;
            else if (_method == HttpMethod.Connect)
                return ProxyProtocol.TunnelProxy;
            return ProxyProtocol.HttpProxy;
        }
    }
    
    public AuthenticationHeaderValue? ProxyAuthorization { get; private set; }
    
    /// <summary>
    /// 此方法会在HttpParse解析完请求行之后触发
    /// 这个时候根据解析的信息来填充feature里的内容
    /// </summary>
    /// <param name="versionAndMethod"></param>
    /// <param name="targetPath"></param>
    /// <param name="startLine"></param>
    /// <exception cref="NotImplementedException"></exception>
    void IHttpRequestLineHandler.OnStartLine(HttpVersionAndMethod versionAndMethod, TargetOffsetPathLength targetPath, Span<byte> startLine)
    {
        // 举例请求行: GET http://www.example.com/path/to/resource?query=example&sort=asc HTTP/1.1
        // 举例请求行: CONNECT www.example.com:443 HTTP/1.1
        _method = versionAndMethod.Method;
        var host = Encoding.ASCII.GetString(startLine.Slice(targetPath.Offset, targetPath.Length));
        if (_method == HttpMethod.Connect)
        {
            ProxyHost = HostString.FromUriComponent(host);
        }
        else
        {
            if (Uri.TryCreate(host, UriKind.Absolute, out var uri))
            {
                ProxyHost = HostString.FromUriComponent(uri);
            }
        }
    }
    
    void IHttpHeadersHandler.OnHeader(ReadOnlySpan<byte> name, ReadOnlySpan<byte> value)
    {
        // 举例头部:
        // Host: example.com
        // Proxy-Authorization: Basic dXNlcjpwYXNzd29yZA==
        const string proxyAuthentication = "Proxy-Authorization";
        var headerName = Encoding.ASCII.GetString(name);
        if (proxyAuthentication.Equals(headerName, StringComparison.OrdinalIgnoreCase))
        {
            var headerValue = Encoding.ASCII.GetString(value);
            if (AuthenticationHeaderValue.TryParse(headerValue, out var parsedValue))
            {
                ProxyAuthorization = parsedValue;
            }
        }
    }

    void IHttpHeadersHandler.OnStaticIndexedHeader(int index)
    {
        
    }

    void IHttpHeadersHandler.OnStaticIndexedHeader(int index, ReadOnlySpan<byte> value)
    {
        
    }

    void IHttpHeadersHandler.OnHeadersComplete(bool endStream)
    {
        
    }
}