using System.Runtime.CompilerServices;
using NullGC.Extensions;

namespace NullGC.Allocators;

public class AllocatorSyncWrapper<TInner> : IMemoryAllocator, IDisposable where TInner : class, IMemoryAllocator
{
    public AllocatorSyncWrapper(TInner inner)
    {
        Inner = inner;
    }

    public TInner Inner { get; }

    public object SyncRoot => Inner;

    public UIntPtr Allocate(nuint size)
    {
        lock (Inner) return Inner.Allocate(size);
    }

    public ReallocResult TryRealloc(UIntPtr ptr, UIntPtr minSize, nuint maxSize)
    {
        if (ptr == UIntPtr.Zero) return ReallocResult.NotSuccess;
        
        lock (Inner) return Inner.TryRealloc(ptr, minSize, maxSize);
    }

    public void Free(UIntPtr pointer)
    {
        lock (Inner) Inner.Free(pointer);
    }

    public uint MetadataOverhead => Inner.MetadataOverhead;

    public void ClearCachedMemory()
    {
        lock (Inner)
        {
            if (typeof(TInner).IsAssignableTo(typeof(IAllocatorCacheable)))
                // ReSharper disable once SuspiciousTypeConversion.Global
                ((IAllocatorCacheable)Inner).ClearCachedMemory();
        }
    }

    public void Dispose()
    {
        Inner.TryDispose();
    }
}