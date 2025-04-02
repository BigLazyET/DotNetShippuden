using System.Buffers;

namespace KestrelApp.Common.System.Buffers;

public sealed class ArrayOwner<T> : IArrayOwner<T>
{
    private bool _disposed = false;
    private readonly ArrayPool<T> _arrayPool;
    
    public int Length { get; }
    
    public T[] Array { get; }

    public ArrayOwner(ArrayPool<T> arrayPool, int length)
    {
        Length = length;
        _arrayPool = arrayPool;
        Array = _arrayPool.Rent(length);
    }
    
    public Span<T> AsSpan()
    {
        return Array.AsSpan();
    }

    public Memory<T> AsMemory()
    {
        return Array.AsMemory();
    }

    public void Dispose()
    {
        Dispose(true);
        // 通知垃圾回收机制不再调用终结器
        GC.SuppressFinalize(this);
    }
    
    ~ArrayOwner()
    {
        Dispose(false);
    }

    /// <summary>
    /// 清理
    /// </summary>
    /// <param name="disposing">是否清理托管资源</param>
    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        if (disposing)
        {
            // 清理托管资源，这里是归还到数组池里
            _arrayPool.Return(Array);
        }
        // 清理非托管资源
        _disposed = true;
    }
}