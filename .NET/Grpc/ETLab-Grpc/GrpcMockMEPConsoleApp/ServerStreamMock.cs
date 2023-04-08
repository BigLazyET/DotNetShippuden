using System.IO.Pipelines;
using System.Net;

namespace GrpcMockMEPConsoleApp
{
    public static class ServerStreamMock
    {
        public static async Task HandleServerStreamCallAsync(HttpContext httpContext)
        {
            var reader = httpContext.Request.BodyReader;
            var writer = httpContext.Response.BodyWriter;

            await reader.ReadAndProcessAsync(HelloRequest.Parser, async hello =>
            {
                var names = hello.Names.Split(',');
                foreach (var name in names)
                {
                    var reply = new HelloReply { Message = $"Hello, {name}" };
                    await writer.WriteMessageAsync(reply);
                    await Task.Delay(1000);
                }
            });
        }

        public static async Task ServerStreamCallAsync()
        {
            using var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "")
            {
                Version = HttpVersion.Version20,
                VersionPolicy = HttpVersionPolicy.RequestVersionExact,
                Content = new MessageContent(new HelloRequest { Names = "foo,bar,baz,qux" })
            };
            var reply = await httpClient.SendAsync(request);
            await PipeReader.Create(await reply.Content.ReadAsStreamAsync()).ReadAndProcessAsync(HelloReply.Parser, reply =>
            {
                Console.WriteLine($"{DateTimeOffset.Now}-{reply.Message}");
                return Task.CompletedTask;
            });
        }
    }
}
