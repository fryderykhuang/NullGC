using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using NullGC.Extensions;

namespace NullGC.Allocators;

public class AllocatorPool<T> : IAllocatorPool, IAllocatorProvider, IAllocatorCacheable, IMemoryAllocationTrackable,
    IMemoryAllocationTracker, IDisposable
    where T : class, IMemoryAllocator
{
    private readonly Func<AllocatorPool<T>, T> _factory;

    public AllocatorPool(Func<AllocatorPool<T>, T> factory)
    {
        _factory = factory;
    }

    private readonly Stack<IMemoryAllocator> _pool = new();
    private ulong _clientAllocatedBytes;
    private ulong _clientFreedBytes;

    public IMemoryAllocator Get()
    {
        lock (_pool)
        {
            if (_pool.TryPop(out var arena))
            {
                return arena;
            }
        }

        return _factory(this);
    }

    public void Return(IMemoryAllocator allocator)
    {
        lock (_pool)
        {
            _pool.Push(allocator);
        }
    }

    IMemoryAllocator IAllocatorProvider.GetAllocator()
    {
        return Get();
    }

    public void ClearCachedMemory()
    {
        lock (_pool)
        {
            foreach (var allocator in _pool)
            {
                allocator.TryDispose();
            }
            _pool.Clear();
        }
    }

    public ulong SelfTotalAllocated => 0;

    public ulong SelfTotalFreed => 0;
    public bool IsAllFreed => SelfTotalAllocated == SelfTotalFreed;
    public ulong ClientTotalAllocated => _clientAllocatedBytes;
    public ulong ClientTotalFreed => _clientFreedBytes;
    public bool ClientIsAllFreed => ClientTotalAllocated == ClientTotalFreed;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IMemoryAllocationTracker.ClientAllocate(ulong bytes)
    {
        Interlocked.Add(ref _clientAllocatedBytes, bytes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IMemoryAllocationTracker.ClientFree(ulong bytes)
    {
        Interlocked.Add(ref _clientFreedBytes, bytes);
    }

    public void Dispose()
    {
        lock (_pool)
        {
            while (_pool.TryPop(out var allocator))
            {
                allocator.TryDispose();
            }
        }
    }
}