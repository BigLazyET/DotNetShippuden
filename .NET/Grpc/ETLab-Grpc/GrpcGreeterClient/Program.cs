using Grpc.Net.Client;

namespace GrpcGreeterClient
{
    internal class Program
    {
        const string GrpcGreetAddress = "https://localhost:7040";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            SayHello();
            
            Console.WriteLine("Press Any Key to exit...");
            Console.ReadLine();
        }

        static async void SayHello()
        {
            using var channel = GrpcChannel.ForAddress(GrpcGreetAddress);
            var client = new Greeter.GreeterClient(channel);

            while (true)
            {
                var reply = await client.SayHelloAsync(new HelloRequest { Name = "GrpcGreeterClient" });
                Console.WriteLine($"Greeting: {reply.Message}");
                await Task.Delay(5000);
            }
        }
    }
}