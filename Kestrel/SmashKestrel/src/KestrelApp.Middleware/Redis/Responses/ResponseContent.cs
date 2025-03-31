using System.Text;

namespace KestrelApp.Middleware.Redis;

abstract class ResponseContent
{
    public static ResponseContent Ok { get; } = new StringContent("+OK\r\n");

    public static ResponseContent Err { get; } = new StringContent("+ERR\r\n");
    
    
    public static ResponseContent Pong { get; } = new StringContent("+PONG\r\n");

    public abstract ReadOnlyMemory<byte> ToMemory();
    
    private class StringContent : ResponseContent
    {
        private readonly ReadOnlyMemory<byte> value;
        
        public StringContent(string response)
        {
            value = Encoding.UTF8.GetBytes(response);
        }

        public override ReadOnlyMemory<byte> ToMemory()
        {
            return value;
        }
    }
}