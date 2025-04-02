namespace KestrelApp.Common.System.Buffers;

/// <summary>
/// 数组持有者接口
/// </summary>
public interface IArrayOwner<T> : IDisposable
{
    /// <summary>
    /// 数据长度
    /// </summary>
    int Length { get; }
    
    /// <summary>
    /// 持有的数组
    /// </summary>
    T[] Array { get; }

    /// <summary>
    /// 转换成的Span
    /// </summary>
    Span<T> AsSpan();

    /// <summary>
    /// 转换成的Memory
    /// </summary>
    Memory<T> AsMemory();
}