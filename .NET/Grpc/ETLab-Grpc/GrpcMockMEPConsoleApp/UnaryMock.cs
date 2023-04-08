using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipelines;
using System.Net;
using Microsoft.AspNetCore.Http;
using Google.Protobuf;

namespace GrpcMockMEPConsoleApp
{
    public static class UnaryMock
    {
        public static async Task HandleUnaryCallAsync(HttpContext httpContext)
        {
            var reader = httpContext.Request.BodyReader;
            var writer = httpContext.Response.BodyWriter;

            await reader.ReadAndProcessAsync(HelloRequest.Parser, async hello =>
            {
                var reply = new HelloReply { Message = $"Hello, {hello.Names}" };
                await writer.WriteMessageAsync(reply);
            });
        }

        public static async Task UnaryCallAsync()
        {
            using var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "")
            {
                Version = HttpVersion.Version20,
                VersionPolicy = HttpVersionPolicy.RequestVersionExact,
                Content = new MessageContent(new HelloRequest { Names = "tennis" })
            };
            HttpResponseMessage reply = await httpClient.SendAsync(request);
            await PipeReader.Create(await reply.Content.ReadAsStreamAsync()).ReadAndProcessAsync(HelloReply.Parser, reply =>
            {
                Console.WriteLine(reply.Message);
                return Task.CompletedTask;
            });
        }
    }

    public class MessageContent : HttpContent
    {
        private readonly IMessage message;

        public MessageContent(IMessage message)
        {
            this.message = message;
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context)
        {
            await PipeWriter.Create(stream).WriteMessageAsync(message);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }
    }
}
