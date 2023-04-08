using Google.Protobuf;
using System.IO.Pipelines;
using System.Net;

namespace GrpcMockMEPConsoleApp
{
    /// <summary>
    /// 模拟ClientStream，客户端以流的形式将请求内容提交给服务端处理
    /// 客户端以流的方式发送foo,bar,baz,qux
    /// 服务端直接接收
    /// </summary>
    public static class ClientStreamMock
    {
        public static async Task HandleClientStreamCallAsync(HttpContext httpContext)
        {
            var reader = httpContext.Request.BodyReader;
            var writer = httpContext.Response.BodyWriter;

            await reader.ReadAndProcessAsync(HelloRequest.Parser, async hello =>
            {
                var names = hello.Names.Split(',');
                foreach (var name in names)
                {
                    Console.WriteLine($"[{DateTimeOffset.Now}] hello, {name}");
                    await Task.Delay(1000);
                }
            });
        }

        public static async Task ClientStreamCallAsync()
        {
            using var httpClient = new HttpClient();
            var writer = new ClientStreamWriter<HelloRequest>();
            var request = new HttpRequestMessage(HttpMethod.Post, "")
            {
                Version = HttpVersion.Version20,
                VersionPolicy = HttpVersionPolicy.RequestVersionExact,
                Content = new ClientStreamContent<HelloRequest>(writer)
            };
            var reply = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            foreach (var name in new[] { "foo","bar","baz","qux" })
            {
                await writer.WriteAsync(new HelloRequest { Names = name });
                await Task.Delay(1000);
            }
            writer.Compelete();
        }
    }

    public class ClientStreamWriter<TMessage> where TMessage : IMessage<TMessage>
    {
        private readonly TaskCompletionSource<Stream> streamSetSource = new();
        private readonly TaskCompletionSource streamEndSource = new();

        public ClientStreamWriter<TMessage> SetOutputStream(Stream outputStream)
        {
            streamSetSource.SetResult(outputStream);
            return this;
        }

        public async Task WriteAsync(TMessage message)
        {
            var stream = await streamSetSource.Task;
            await PipeWriter.Create(stream).WriteMessageAsync(message);
        }

        public void Compelete()
        {
            streamEndSource.SetResult();
        }

        public Task WaitAsync()
        {
            return streamEndSource.Task;
        }
    }

    /// <summary>
    /// 自定义HttpContent类型，让其以"客户端流"的形式向对方发送内容
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class ClientStreamContent<TMessage> : HttpContent where TMessage : IMessage<TMessage>
    {
        private readonly ClientStreamWriter<TMessage> clientStreamWriter;

        public ClientStreamContent(ClientStreamWriter<TMessage> clientStreamWriter)
        {
            this.clientStreamWriter = clientStreamWriter;
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
        {
            return clientStreamWriter.SetOutputStream(stream).WaitAsync();
        }

        protected override bool TryComputeLength(out long length)
        {
            return (length = -1) != -1;
        }
    }
}
