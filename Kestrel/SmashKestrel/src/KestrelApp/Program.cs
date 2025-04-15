using System.Net;
using KestrelApp.Middleware.Echo;
using KestrelApp.Middleware.HttpProxy;
using KestrelApp.Middleware.HttpProxy.Extensions;
using KestrelApp.Middleware.Redis;
using KestrelApp.Middleware.Telnet;
using Microsoft.AspNetCore.Server.Kestrel;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddConnections().AddHttpProxy().AddTelnet().AddRedis();

// configure kestrel middlewares
// 可以让kestrel使用一个端口支持多种协议或多协议一个端口一种协议的要求
builder.WebHost.ConfigureKestrel((WebHostBuilderContext context, KestrelServerOptions options) =>
{
    // 使用配置，可以很方便的配置多种协议: 一个端口一种协议的要求，如下示例所示
    var kestrelSection = context.Configuration.GetSection("Kestrel");
    options.Configure(kestrelSection)
        .Endpoint(IPAddress.Parse("127.0.0.1"), 8945)
        .Endpoint("HttpProxy", endpoint => endpoint.ListenOptions.UseHttpProxy())
        .Endpoint("Telnet", endpoint => endpoint.ListenOptions.UseTelnet())
        .Endpoint("Echo", (endpoint) => endpoint.ListenOptions.UseEcho())
        .Endpoint("Redis", (EndpointConfiguration endpoint) => endpoint.ListenOptions.UseRedis());

    // 手动配置
    // options.Configure().Endpoint(IPAddress.Parse("127.0.0.1"), 5007, listenOptions =>
    // {
    //     listenOptions.UseRedis();
    // });
});

var app = builder.Build();
app.UseRouting();

// http代理中间件，能处理非隧道的http代理请求
app.UseMiddleware<HttpProxyMiddleware>();

app.MapConnectionHandler<TelnetConnectionHandler>("/telnet");

app.Map("/", async context =>
{
    await context.Response.WriteAsync("Hello World");
});

app.Run();