using System.Net;
using KestrelApp.Middleware.Redis;
using Microsoft.AspNetCore.Server.Kestrel;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRedis();

// configure kestrel middlewares
// 可以让kestrel使用一个端口支持多种协议或多协议一个端口一种协议的要求
builder.WebHost.ConfigureKestrel((WebHostBuilderContext context, KestrelServerOptions options) =>
{
    // 使用配置，可以很方便的配置多种协议: 一个端口一种协议的要求，如下示例所示
    var kestrelSection = context.Configuration.GetSection("Kestrel");
    options.Configure(kestrelSection)
           .Endpoint("Redis", (EndpointConfiguration endpoint) => endpoint.ListenOptions.UseRedis());
    
    // 手动配置
    // options.Configure().Endpoint(IPAddress.Parse("127.0.0.1"), 5007, listenOptions =>
    // {
    //     listenOptions.UseRedis();
    // });
});

var app = builder.Build();
app.UseRouting();

app.Map("/", async context =>
{
    context.Response.ContentType = "text/html";
    await context.Response.SendFileAsync("telnet-websocket.html");
});

app.Run();