using System.Text;

namespace KestrelApp.Middleware.Redis;

public class RedisValue
{
    public ReadOnlyMemory<byte> Value { get; }

    public RedisValue(ReadOnlyMemory<byte> value) => Value = value;

    public static explicit operator string(RedisValue value)
    {
        return Encoding.UTF8.GetString(value.Value.Span);
    }
    
    public override string ToString() => Encoding.UTF8.GetString(Value.Span);
}