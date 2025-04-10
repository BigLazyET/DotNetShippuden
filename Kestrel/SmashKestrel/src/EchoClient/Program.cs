using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO.Pipelines;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Text;

const string Host = "localhost";
const int Port = 5006;

using var tcpClient = new TcpClient(Host, Port);
var ns = tcpClient.GetStream();

var writePipe = new Pipe();
var readPipe = new Pipe();

var random = new Random();

var cts = new CancellationTokenSource();

// 启动任务来将管道数据复制到网络流
var writeTask = Task.Run(async () =>
{
    try
    {   
        await writePipe.Reader.CopyToAsync(ns, cts.Token);
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"Write task error: {ex.Message}");
    }
    finally
    {
        await writePipe.Reader.CompleteAsync();
    }
}, cts.Token);

var readTask = Task.Run(async () =>
{
    try
    {
        await ns.CopyToAsync(readPipe.Writer, cts.Token);
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"Read task error: {ex.Message}");
    }
    finally
    {
        await readPipe.Writer.CompleteAsync();
    }
}, cts.Token);

var produceTask = Task.Run(async () =>
{
    try
    {
        while (!cts.Token.IsCancellationRequested)
        {
            var sendIndex = random.Next(0, 9);
            var msg = $"Hello World {sendIndex}";
            var bytes = Encoding.UTF8.GetBytes(msg);

            const int size = sizeof(short); // 2个字节
            var span = writePipe.Writer.GetSpan(size);
            BinaryPrimitives.WriteInt16BigEndian(span, (short)bytes.Length);
            writePipe.Writer.Advance(size);
            await writePipe.Writer.WriteAsync(bytes, cts.Token);
            await writePipe.Writer.FlushAsync(cts.Token);
            Debug.WriteLine($"发送给Echo服务器的消息: {msg}");
            
            await Task.Delay(1000, cts.Token);
        }
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"Produce task error: {ex.Message}");
    }
    finally
    {
        await writePipe.Writer.CompleteAsync();
    }
}, cts.Token);

var printTask = Task.Run(async () =>
{
    try
    {
        while (!cts.Token.IsCancellationRequested)
        {
            var result = await readPipe.Reader.ReadAsync(cts.Token);
            var buffer = result.Buffer;
            SequencePosition? position = null;

            while (buffer.Length > 0)
            {
                var reader = new SequenceReader<byte>(buffer);
                if (!reader.TryReadBigEndian(out short length) || reader.Remaining < length)
                {
                    break;
                }

                var messageBytes = reader.Sequence.Slice(reader.Position, length).ToArray();
                var msg = Encoding.UTF8.GetString(messageBytes);
                Debug.WriteLine($"从Echo服务器返回的消息: {msg}");

                reader.Advance(length);
                position = reader.Position;
                buffer = buffer.Slice(reader.Position);
            }

            if (position != null)
            {
                readPipe.Reader.AdvanceTo(position.Value);
            }
            else
            {
                readPipe.Reader.AdvanceTo(buffer.Start, buffer.End);
            }

            if (result.IsCompleted)
                break;
        }
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"Print task error: {ex.Message}");
    }
    finally
    {
        await readPipe.Reader.CompleteAsync();
    }
}, cts.Token);

await Task.WhenAll(writeTask, readTask, produceTask, printTask);
Debug.WriteLine("echo 结束");