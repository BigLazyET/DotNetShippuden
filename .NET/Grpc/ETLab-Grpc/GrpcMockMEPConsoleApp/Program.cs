namespace GrpcMockMEPConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)

        {
            var app = WebApplication.Create(args);

            app.MapPost("/unary", UnaryMock.HandleUnaryCallAsync);
            app.MapPost("/serverstream", ServerStreamMock.HandleServerStreamCallAsync);
        }
    }
}