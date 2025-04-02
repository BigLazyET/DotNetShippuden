using System.Buffers;

namespace KestrelApp.Common.System.Buffers;

public static class ArrayPoolExtensions
{
    public static ArrayOwner<byte> RentArrayOwner(this ArrayPool<byte> arrayPool, int length)
    {
        return new ArrayOwner<byte>(arrayPool, length);
    }
}