using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace KestrelApp.Common.System.Buffers;

public static class BufferWriterExtensions
{
    public unsafe static int Write(this IBufferWriter<byte> writer, ReadOnlySpan<char> text, Encoding encoding)
    {
        if (text.IsEmpty)
            return 0;

        var chars = (char*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(text));
        var byteCount = encoding.GetByteCount(chars, text.Length);

        var span = writer.GetSpan(byteCount);
        var bytes = (byte*)Unsafe.AsPointer(ref span[0]);
        // 上一行等同于: var bytes = (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
        
        // 使用编码器将字符转换为字节并写入到 bytes 指针指向的内存中，flush: true 表示刷新编码器的状态。
        var len = encoding.GetEncoder().GetBytes(chars, text.Length, bytes, byteCount, true);
        writer.Advance(len);

        return len;
    }

    public static void WriteBigEndian(this IBufferWriter<byte> writer, ushort value)
    {
        const int size = sizeof(ushort);
        var span = writer.GetSpan(size);
        BinaryPrimitives.WriteUInt16BigEndian(span, value);
        writer.Advance(size);
    }
}