using System.Runtime.CompilerServices;

namespace NullGC.Collections;

public static class UnsafeHelper
{
    public static unsafe void InitBlock(void* startAddress, byte value, nuint byteCount)
    {
        while (byteCount > 0)
        {
            var bc = (uint) Math.Min(byteCount, (nuint) uint.MaxValue);
            Unsafe.InitBlock(startAddress, value, bc);
            byteCount -= bc;
        }
    }
    
    public static unsafe void InitBlockUnaligned(void* startAddress, byte value, nuint byteCount)
    {
        while (byteCount > 0)
        {
            var bc = (uint) Math.Min(byteCount, (nuint) uint.MaxValue);
            Unsafe.InitBlockUnaligned(startAddress, value, bc);
            byteCount -= bc;
        }
    }

    public static unsafe void CopyBlock(void* destination, void* source, nuint byteCount)
    {
        while (byteCount > 0)
        {
            var bc = (uint) Math.Min(byteCount, (nuint) uint.MaxValue);
            Unsafe.CopyBlock(destination, source, bc);
            byteCount -= bc;
            destination = (byte*)destination + bc;
            source = (byte*)source + bc;
        }
    }
    
    public static unsafe void CopyBlockUnaligned(void* destination, void* source, nuint byteCount)
    {
        while (byteCount > 0)
        {
            var bc = (uint) Math.Min(byteCount, (nuint) uint.MaxValue);
            Unsafe.CopyBlockUnaligned(destination, source, bc);
            byteCount -= bc;
            destination = (byte*)destination + bc;
            source = (byte*)source + bc;
        }
    }
}