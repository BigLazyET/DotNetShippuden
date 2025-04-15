// See https://aka.ms/new-console-template for more information

using System.Net.Sockets;

await Task.Delay(10000);

const string Host = "localhost";
const int Port = 5000;

var cts = new CancellationTokenSource();

var tcpClient = new TcpClient(Host, Port) { ReceiveTimeout = 10000, SendTimeout = 10000 };
var stream = tcpClient.GetStream();
var streamReader = new StreamReader(stream);
var streamWriter = new StreamWriter(stream);// { AutoFlush = true };

var produceTask = Task.Run(async () =>
{
    while (cts.IsCancellationRequested == false)
    {
        var input = Console.ReadLine();
        await streamWriter.WriteAsync(input + "\r\n");
        await streamWriter.FlushAsync(); // 开启AutoFlush则不用设置此行
    }
}, cts.Token);

var consumeTask = Task.Run(async () =>
{
    while (cts.IsCancellationRequested == false)
    {
        var output = await streamReader.ReadLineAsync();
        Console.WriteLine($"telnet says: {output}");
    }
});

await Task.WhenAny(produceTask, consumeTask);
tcpClient.Dispose();
stream.Dispose();
streamReader.Dispose();
streamWriter.Dispose();
Console.WriteLine("结束");