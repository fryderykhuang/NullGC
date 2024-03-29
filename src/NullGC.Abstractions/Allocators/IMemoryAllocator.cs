﻿using System.Runtime.InteropServices;

namespace NullGC.Allocators;

public static class MemoryConstants
{
    public static readonly uint DefaultAlignment = (uint) (Environment.Is64BitProcess ? 16 : 8);
// #if TARGET_64BIT
//             16
// #else
//             8
// #endif
//         ;
}

public interface IMemoryAllocator
{
    UIntPtr Allocate(nuint size);

    /// <summary>
    /// Try resize the memory without altering the existing data.
    /// </summary>
    /// <param name="ptr"></param>
    /// <param name="minSize"></param>
    /// <param name="maxSize"></param>
    /// <returns>If return value is zero, resize is not successful, original data in <paramref name="ptr"/> is still preserved, otherwise resize is successful.</returns>
    ReallocResult TryRealloc(UIntPtr ptr, nuint minSize, nuint maxSize);

    void Free(UIntPtr pointer);

    /// <summary>
    /// If your allocator needs to store metadata on the buffer before the address of returned pointer, this specifies the byte size.     
    /// </summary>
    uint MetadataOverhead { get; }
}