using KestrelApp.Middleware.Redis;
using Microsoft.AspNetCore.Server.Kestrel;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// configure kestrel middlewares
// 可以让kestrel使用一个端口支持多种协议或多协议一个端口一种协议的要求
builder.WebHost.ConfigureKestrel((WebHostBuilderContext context, KestrelServerOptions options) =>
{
    var kestrelSection = context.Configuration.GetSection("Kestrel");
    options.Configure(kestrelSection)
           .Endpoint("Redis", (EndpointConfiguration endpoint) => endpoint.ListenOptions.UseRedis());
});


// var app = builder.Build();
//
// app.MapGet("/", () => "Hello World!");
//
// app.Run();