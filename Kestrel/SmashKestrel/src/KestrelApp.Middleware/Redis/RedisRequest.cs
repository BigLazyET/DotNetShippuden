using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace KestrelApp.Middleware.Redis;

/// <summary>
/// 表示Reids单个请求的完整命令
/// 比如LPUSH mylist value
/// </summary>
public class RedisRequest
{
    private readonly List<RedisValue> Values = new();
    
    /// <summary>
    /// 数据包大小
    /// </summary>
    public int Size { get; private set; }
    
    /// <summary>
    /// 命令名称
    /// </summary>
    public RedisCmd Cmd { get; private set; }

    public int ArgumentCount => Values.Count - 1;   // 将Cmd 比如：LPUSH排除在外; 只有mylist value是参数

    /// <summary>
    /// 获取命令参数
    /// </summary>
    /// <param name="index">索引值</param>
    /// <returns></returns>
    public RedisValue Argument(int index)
    {
        return Values[index + 1];
    }
    
    public static IList<RedisRequest> Parse(ReadOnlySequence<byte> buffer, out SequencePosition consumed)
    {
        var memory = buffer.First;
        if (buffer.IsSingleSegment == false)
        {
            memory = buffer.ToArray().AsMemory();
        }

        var size = 0;
        var requestList = new List<RedisRequest>();
        
        while (TryParse(memory, out var request))
        {
            size += request.Size;
            requestList.Add(request);
            memory = memory.Slice(request.Size);
        }

        consumed = buffer.GetPosition(size);
        return requestList;
    }

    /// <summary>
    /// 根据请求内容解析生成RedisRequest
    /// </summary>
    /// <param name="memory"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="RedisProtocolException"></exception>
    private static bool TryParse(ReadOnlyMemory<byte> memory, [MaybeNullWhen(false)] out RedisRequest request)
    {
        // 一个基于RESP（REdis Serialization Protocol）协议的例子
        // 命令：LPUSH mylist value 对应的RESP格式:
        // *3\r\n
        // $5\r\n
        // LPUSH\r\n
        // $6\r\n
        // mylist\r\n
        // $5\r\n
        // value\r\n
        request = default;
        if (memory.IsEmpty == true)
            return false;

        var span = memory.Span;
        if (span[0] != '*')
            throw new RedisProtocolException();

        if (span.Length < 4)
            return false;

        var lineLength = span.IndexOf((byte)'\n') + 1;
        if (lineLength < 4) // 最小需要四个字节：*1\r\n
            throw new RedisProtocolException();

        var lineCountSpan = span.Slice(1, lineLength - 3);
        var lineCountString = Encoding.ASCII.GetString(lineCountSpan);
        if (int.TryParse(lineCountString, out var lineCount) == false || lineCount < 0)
            throw new RedisProtocolException();

        request = new RedisRequest();
        span = span.Slice(lineLength);
        for (int i = 0; i < lineCount; i++)
        {
            if (span[0] != '$')
                throw new RedisProtocolException();
            lineLength = span.IndexOf((byte)'\n') + 1;
            if (lineLength < 4)
                throw new RedisProtocolException();
            var lineContentLengthSpan = span.Slice(1, lineLength - 3);
            var lineContentLengthString = Encoding.ASCII.GetString(lineContentLengthSpan);
            if (int.TryParse(lineContentLengthString, out var lineContentLength) == false)
                throw new RedisProtocolException();

            span = span.Slice(lineLength);
            if (span.Length < lineContentLength + 2)
                return false;
            var lineContentBytes = span.Slice(0, lineContentLength).ToArray();
            var redisValue = new RedisValue(lineContentBytes);
            request.Values.Add(redisValue);

            span = span.Slice(lineContentLength + 2);
        }

        request.Size = memory.Span.Length - span.Length;
        Enum.TryParse<RedisCmd>(request.Values[0].ToString(), false, out var cmd);
        request.Cmd = cmd;
        return true;
    }

    public override string ToString() => string.Join(',', Values);
}