﻿using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NullGC.Diagnostics;

namespace NullGC.Allocators;

/// <summary>
/// Native memory allocator with alignment defaults to <see cref="MemoryConstants.DefaultAlignment"/> 
/// </summary>
public class DefaultAlignedNativeMemoryAllocator : IMemoryAllocator, IAllocatorProvider
{
    public static DefaultAlignedNativeMemoryAllocator Default { get; } = new();

    /// <inheritdoc />
    public UIntPtr Allocate(nuint size)
    {
        unsafe
        {
            var ptr = (UIntPtr) NativeMemory.AlignedAlloc(size, MemoryConstants.DefaultAlignment);
            TraceOn.MemAlloc(nameof(DefaultAlignedNativeMemoryAllocator),
                $"Allocated {ptr:X} with size {size:N} [{MemoryConstants.DefaultAlignment}]");
            Debug.Assert(ptr % MemoryConstants.DefaultAlignment == 0);
            return ptr;
        }
    }

    /// <inheritdoc />
    public ReallocResult TryRealloc(UIntPtr ptr, nuint minSize, nuint maxSize)
    {
        if (ptr == UIntPtr.Zero) return new ReallocResult();

        unsafe
        {
#if TRACE_MEM_ALLOC
            var oldPtr = ptr;
            TraceOn.MemAlloc(nameof(DefaultAlignedNativeMemoryAllocator), $"Before resize {oldPtr:X} to {minSize:N} {maxSize:N} [{MemoryConstants.DefaultAlignment}]");
#endif
            Debug.Assert(ptr % MemoryConstants.DefaultAlignment == 0);
            ptr = (UIntPtr) NativeMemory.AlignedRealloc(ptr.ToPointer(), maxSize, MemoryConstants.DefaultAlignment);
            Debug.Assert(ptr % MemoryConstants.DefaultAlignment == 0);
            
#if TRACE_MEM_ALLOC
            TraceOn.MemAlloc(nameof(DefaultAlignedNativeMemoryAllocator),
                $"Resized {oldPtr:X} to {ptr:X} {minSize:N} {maxSize:N} [{MemoryConstants.DefaultAlignment}]");
#endif
        }

        return new ReallocResult(ptr, maxSize);
    }


    public IMemoryAllocator GetAllocator() => this;

    /// <inheritdoc />
    public void Free(UIntPtr ptr)
    {
        unsafe
        {
            TraceOn.MemAlloc(nameof(DefaultAlignedNativeMemoryAllocator), $"Before free {ptr:X}");
            Debug.Assert(ptr % MemoryConstants.DefaultAlignment == 0);
            NativeMemory.AlignedFree((void*) ptr);
        }
    }

    public uint MetadataOverhead => 0;
}