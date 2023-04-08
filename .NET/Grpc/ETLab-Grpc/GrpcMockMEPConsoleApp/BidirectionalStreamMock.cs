using System.IO.Pipelines;
using System.Net;

namespace GrpcMockMEPConsoleApp
{
    public static class BidirectionalStreamMock
    {
        public static async async BidirectionalStreamCallAsync()
        {
            using var httpClient = new HttpClient();
            var writer = new ClientStreamWriter<HelloRequest>();
            var request = new HttpRequestMessage(HttpMethod.Post, "")
            {
                Version = HttpVersion.Version20,
                VersionPolicy = HttpVersionPolicy.RequestVersionExact,
                Content = new ClientStreamContent<HelloRequest>(writer)
            };
            var task = httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            _ = Task.Run(async () =>
            {
                var response = await task;
                await PipeReader.Create(await response.Content.ReadAsStreamAsync())
                                .ReadAndProcessAsync(HelloReply.Parser, reply =>
                                {
                                    Console.WriteLine($"[{DateTimeOffset.Now}] {reply.Message}");
                                    return Task.CompletedTask;
                                });
            });

            foreach (var name in new[] { "foo", "bar", "baz", "qux" })
            {
                await writer.WriteAsync(new HelloRequest { Names = name });
                await Task.Delay(1000);
            }
            writer.Compelete();
        }
    }
}
