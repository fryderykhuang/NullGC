using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;
using NullGC.Collections;
using NullGC.Diagnostics;
using UIntPtr = System.UIntPtr;

namespace NullGC.Allocators;

/// <summary>
///     Pooler for another allocator, make repeated allocate/free cycles faster.
///     At the same time works as a pooled end user allocator.
/// </summary>
/// <remarks>
///     All public methods are synced, performance will degrade on heavy concurrent use.
///     This class uses LRU and dynamic TTL to make evictions.
/// </remarks>
public class DefaultAllocationPooler : IMemoryAllocator, IMemoryAllocationTrackable, IAllocatorCacheable,
    IAllocatorProvider
{
    private readonly int _consecutivePrunesBeforeAdjustTh;
    private readonly IMemoryAllocator _inner;
    private readonly uint _overhead;
    private readonly nuint _poolItemSizeLimit;
    private readonly int _cacheLostObserveWindowSize;


    private readonly int _ttl;
    private nuint _cleanupThreshold;
    private ulong _clientAllocated;
    private ulong _clientFreed;

    private int _consecutivePrunes;
    private ulong _currentPoolSize;
    private ValueLinkedList<LruItem> _lruList = new(8, (int) AllocatorTypes.DefaultUncachedUnscoped);

    private ValueDictionary<nuint, AllocationPool> _pool = new(8, (int) AllocatorTypes.DefaultUncachedUnscoped);
    private ulong _totalAllocatedBytes;
    private ulong _totalFreedBytes;

    /// <summary>
    /// </summary>
    /// <param name="inner">The actual native memory allocator.</param>
    /// <param name="ttlMs">The pooled memory will become expired if this time passed after returning to the pool.</param>
    /// <param name="cleanupThresholdBytes">
    ///     The threshold over cached bytes before every call to Free() tries to prune expired
    ///     caches.
    /// </param>
    /// <param name="consecutivePrunesBeforeAdjustTh">
    ///     When pruning is triggered consecutively for over this times, the
    ///     cleanup threshold will be adjusted to current cached bytes.
    /// </param>
    /// <param name="poolItemSizeLimit"></param>
    /// <param name="cacheLostObserveWindowSize"></param>
    public DefaultAllocationPooler(IMemoryAllocator inner, int ttlMs = 10000,
        nuint cleanupThresholdBytes = 8 * 1024 * 1024, int consecutivePrunesBeforeAdjustTh = 100,
        nuint poolItemSizeLimit = 5 * 1024 * 1024, int cacheLostObserveWindowSize = 100)
    {
        _inner = inner;
        _ttl = ttlMs;
        _cleanupThreshold = cleanupThresholdBytes;
        _consecutivePrunesBeforeAdjustTh = consecutivePrunesBeforeAdjustTh;
        _poolItemSizeLimit = poolItemSizeLimit;
        _cacheLostObserveWindowSize = cacheLostObserveWindowSize;
        unsafe
        {
            _overhead = MemoryMath.Ceiling((uint) sizeof(Header), MemoryConstants.DefaultAlignment);
        }
    }

    /// <summary>
    ///     Remove all cached memory.
    /// </summary>
    public void ClearCachedMemory()
    {
        lock (_inner)
        {
            foreach (ref var pool in _pool.Values) Clear(ref pool);
        }
    }

    public IMemoryAllocator GetAllocator() => this;

    public ulong SelfTotalAllocated => _totalAllocatedBytes;
    public ulong SelfTotalFreed => _totalFreedBytes;
    public bool IsAllFreed => SelfTotalAllocated == SelfTotalFreed;
    public ulong ClientTotalAllocated => _clientAllocated;
    public ulong ClientTotalFreed => _clientFreed;
    public bool ClientIsAllFreed => ClientTotalAllocated == ClientTotalFreed;

    public UIntPtr Allocate(nuint size)
    {
        lock (_inner)
        {
            UIntPtr ret;
            Guard.IsGreaterThan(size, 0u);
            var allocSize = GetAllocSize(size);
            // {
            //     var ptr = AllocateFromInnerAllocator(allocSize);
            //     Header.WriteToBuffer(ptr, allocSize, size);
            //     ret = ptr + _overhead;
            //     return ret;
            // }
            _clientAllocated += size;
            ref var pool = ref _pool.GetValueRefOrNullRef(allocSize, out var exists);
            if (exists && pool.TryRentFromPool(out var poolItem))
            {
                Debug.Assert(pool.HeadInLruList != -1);
                ref var node = ref _lruList.GetNodeRefUnchecked(pool.HeadInLruList);
                Debug.Assert(node.Value.Count > 0);
                if ((--node.Value.Count) == 0)
                {
                    Debug.Assert(pool.Count == 0);
                    pool.HeadInLruList = node.Value.NextInSameBucket;
                    _lruList.Remove(node.Index);
                }

                Debug.Assert(_currentPoolSize >= allocSize);
                _currentPoolSize -= allocSize;
                new Header(poolItem.FreeablePointer, allocSize, size).WriteToBuffer();
                ret = poolItem.FreeablePointer + _overhead;
            }
            else
            {
                var ptr = AllocateFromInnerAllocator(allocSize);
                new Header(ptr, allocSize, size).WriteToBuffer();
                ret = ptr + _overhead;
            }

            Debug.Assert(ret % MemoryConstants.DefaultAlignment == 0);
            return ret;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReallocResult TryRealloc(UIntPtr ptr, nuint minSize, nuint maxSize)
    {
        TraceOn.MemAlloc(nameof(DefaultAllocationPooler),
            $"Before try realloc {ptr:X} {minSize} {maxSize}");
        if (ptr == UIntPtr.Zero) return ReallocResult.NotSuccess;

        lock (_inner)
        {
            ref var header = ref Header.FromPointer(ptr - _overhead);
            if (header.AllocSize < GetExactFitSize(minSize))
            {
                TraceOn.MemResizeMiss(nameof(DefaultAllocationPooler),
                    $"Miss on {ptr:X} {minSize} {maxSize}");
                return ReallocResult.NotSuccess;
            }

            if (maxSize > minSize)
                minSize = Math.Min(header.AllocSize - _overhead, maxSize);

            var delta = minSize - header.RequestedSize;
            header.RequestedSize = minSize;
            if (delta > 0)
                _clientAllocated += delta;
            else
                _clientFreed += delta;

            TraceOn.MemResizeMiss(nameof(DefaultAllocationPooler), $"Hit on {ptr:X} {minSize} {maxSize}");
            return new ReallocResult(ptr, minSize);
        }
    }

    public uint MetadataOverhead => _overhead;

    public void Free(UIntPtr ptr)
    {
        lock (_inner)
        {
            ref var alloc = ref Header.FromPointer(ptr - _overhead);
            TraceOn.MemAlloc(nameof(DefaultAllocationPooler),
                $"Before free {ptr:X} size={alloc.RequestedSize} allocSiz={alloc.AllocSize}");
            Debug.Assert(alloc.FreeablePointer % MemoryConstants.DefaultAlignment == 0);
            _clientFreed += alloc.RequestedSize;
            var allocSize = alloc.AllocSize;
            if (allocSize <= _poolItemSizeLimit)
            {
                if (_currentPoolSize + allocSize > _cleanupThreshold) Prune(allocSize);

                ref var p = ref _pool.GetValueRefOrAddDefault(allocSize, out var exists);
                if (!exists)
                    p = new AllocationPool(_ttl, _cacheLostObserveWindowSize
#if TRACE_MEM_CACHE_LOST
                        , allocSize
#endif
                    );
                if (p.HeadInLruList == -1)
                {
                    _lruList.AddFront(new LruItem(allocSize));
                    p.HeadInLruList = _lruList.HeadRefUnchecked.Index;
                }
                else
                {
                    ref var node = ref _lruList.GetNodeRefUnchecked(p.HeadInLruList);
                    if (_lruList.IsFirst(p.HeadInLruList))
                    {
                        node.Value.Count++;
                    }
                    else
                    {
                        _lruList.AddFront(new LruItem(allocSize));
                        _lruList.HeadRefUnchecked.Value.NextInSameBucket = p.HeadInLruList;
                        p.HeadInLruList = _lruList.HeadRefUnchecked.Index;
                    }
                }

                ReturnToPool(ref p, alloc);
                _currentPoolSize += allocSize;
            }
            else
            {
                FreeToInnerAllocator(alloc.FreeablePointer, allocSize);
            }
        }
    }

    private int Clear(ref AllocationPool pool)
    {
        int c = 0;
        for (var i = pool.Allocations.Count - 1; i >= 0; i--)
        {
            var alloc = pool.Allocations.GetUnchecked(i);
            FreeToInnerAllocator(alloc.FreeablePointer, alloc.AllocSize);
            _currentPoolSize -= alloc.AllocSize;
            pool.Allocations.RemoveAt(i);
            c++;
        }

        var next = pool.HeadInLruList;
        while (next != -1)
        {
            ref var node = ref _lruList.GetNodeRefUnchecked(next);
            next = node.Value.NextInSameBucket;
            _lruList.Remove(node.Index);
        }

        pool.HeadInLruList = -1;

        return c;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private UIntPtr AllocateFromInnerAllocator(nuint bytes)
    {
        _totalAllocatedBytes += bytes;
        return _inner.Allocate(bytes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FreeToInnerAllocator(UIntPtr ptr, nuint bytes)
    {
        _totalFreedBytes += bytes;
        _inner.Free(ptr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal nuint GetAllocSize(nuint size)
    {
        Debug.Assert(size > 0);
        return MemoryMath.Ceiling(size + _overhead,
            MemoryConstants.DefaultAlignment * ((uint) BitOperations.Log2(size) + 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private nuint GetExactFitSize(nuint size)
    {
        return MemoryMath.Ceiling(size + _overhead, MemoryConstants.DefaultAlignment);
    }

    /// <summary>
    ///     Immediately remove expired cached memory.
    /// </summary>
    /// <param name="cleanupThreshold">Override current cleanup threshold.</param>
    public void Prune(uint? cleanupThreshold = null)
    {
        lock (_inner)
        {
            Prune(0, cleanupThreshold);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int PruneExpiredByPool(ref AllocationPool pool, int pruneCountLimit)
    {
        unsafe
        {
            var c = 0;
            var now = Environment.TickCount64;
            var items = pool.Allocations.Items;
            for (var i = pool.Allocations.Count - 1; i >= 0; i--)
            {
                if (now - items[i].LastReuseTime > pool.Ttl)
                {
                    var allocSize = items[i].AllocSize;
                    FreeToInnerAllocator(items[i].FreeablePointer, allocSize);
                    _currentPoolSize -= allocSize;
                    pool.Allocations.RemoveAt(i);
                    if (++c >= pruneCountLimit) break;
                }
            }

            pool.IncreaseRemoveCount(c);
            return c;
        }
    }

    private void Prune(nuint incomingSize, nuint? cleanupThreshold = null)
    {
        var defaultTh = cleanupThreshold == null;
        cleanupThreshold ??= _cleanupThreshold;
        ref var node = ref _lruList.TailRefOrNullRef;
        while (_currentPoolSize + incomingSize > cleanupThreshold && _lruList.Count > 0)
        {
            ref var pool = ref _pool.GetValueRef(node.Value.AllocSize);
            var pruned = PruneExpiredByPool(ref pool, node.Value.Count);
            if (pruned < node.Value.Count) // no need to walk upwards since anything above is newer.
            {
                node.Value.Count -= pruned;
                break;
            }

            node.Value.Count -= pruned;
            var prev = node.Previous;
            if (node.Value.Count == 0)
            {
                if (pool.HeadInLruList == node.Index) pool.HeadInLruList = node.Value.NextInSameBucket;
                _lruList.Remove(node.Index);
            }

            if (prev == -1) break;

            node = ref _lruList.GetNodeRefUnchecked(prev);
        }

        if (defaultTh && _currentPoolSize + incomingSize > cleanupThreshold)
        {
            Debug.WriteLine(
                $"cleanup threshold is too low, should be greater or equal to {checked((nuint) (_currentPoolSize + incomingSize))}, otherwise TTL can be adjusted shorter.");
            if (++_consecutivePrunes > _consecutivePrunesBeforeAdjustTh)
            {
                // adjust cleanup threshold to try to avoid repeated cleaning up for subsequent Free() calls.
                _cleanupThreshold = checked((nuint) (_currentPoolSize + incomingSize));
                Debug.WriteLine($"Adjusted cleanup threshold to {_cleanupThreshold}");
            }
        }
        else
        {
            _consecutivePrunes = 0;
        }
    }

    private struct LruItem
    {
        public LruItem(nuint allocSize)
        {
            AllocSize = allocSize;
            Count = 1;
        }

        public readonly nuint AllocSize;
        public int Count;

        /// <summary>
        ///     Next linked list index in the same size bucket.
        /// </summary>
        public int NextInSameBucket = -1;
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct PoolItem
    {
        public PoolItem(UIntPtr freeablePointer, UIntPtr allocSize, long lastReuseTime)
        {
            FreeablePointer = freeablePointer;
            AllocSize = allocSize;
            LastReuseTime = lastReuseTime;
        }

        public readonly UIntPtr FreeablePointer;
        public readonly nuint AllocSize;
        public readonly long LastReuseTime;
    }

    private struct Header
    {
        public readonly UIntPtr FreeablePointer;
        public readonly nuint AllocSize;
        public nuint RequestedSize;

        public Header(UIntPtr freeablePointer, nuint allocSize, nuint requestedSize)
        {
            FreeablePointer = freeablePointer;
            AllocSize = allocSize;
            RequestedSize = requestedSize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref Header FromPointer(UIntPtr ptr)
        {
            unsafe
            {
                return ref Unsafe.AsRef<Header>(ptr.ToPointer());
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteToBuffer()
        {
            unsafe
            {
                fixed (void* ptr = &this)
                    Unsafe.CopyBlockUnaligned((void*) FreeablePointer, ptr, (uint) sizeof(Header));
            }
        }
    }

    private struct AllocationPool
    {
        private readonly int _maxTtlAdaptStepMs;

        // TODO Use Deque instead
        public ValueList<PoolItem> Allocations = new(8, (int) AllocatorTypes.DefaultUncachedUnscoped);
        private SlidingWindow<int> _cacheLostObserver;

        public int HeadInLruList = -1;
        private int _removeCount;
        private int _ttl;
        private readonly int _ttlDecreaseStep;
#if TRACE_MEM_CACHE_LOST
        private readonly nuint _size;
#endif

        public void IncreaseRemoveCount(int c) => _removeCount += c;

        public AllocationPool(int ttl, int cacheLostObserveWindowSize
#if TRACE_MEM_CACHE_LOST
            , nuint size
#endif
        )
        {
            _ttl = ttl;
#if TRACE_MEM_CACHE_LOST
            _size = size;
#endif
            _maxTtlAdaptStepMs = ttl * 2;
            _ttlDecreaseStep = ttl / 10;
            _cacheLostObserver = new SlidingWindow<int>(cacheLostObserveWindowSize,
                (int) AllocatorTypes.DefaultUncachedUnscoped);
        }

        public readonly int Count => Allocations.Count;

        public readonly int Ttl => _ttl;

        public bool TryRentFromPool(out PoolItem ret)
        {
            if (Count > 0)
            {
                AdjustTtl(_cacheLostObserver.Update(0));
                var lastIdx = Allocations.Count - 1;
                ret = Allocations.GetUnchecked(lastIdx);
                Allocations.RemoveAt(lastIdx);
                return true;
            }
            else
            {
                if (_removeCount-- > 0) AdjustTtl(_cacheLostObserver.Update(1));
                ret = default;
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AdjustTtl(SlidingWindow<int>.CurrentWindow w)
        {
            if (w.Count < w.Window.Capacity) return;

            if (w.Sum == 0) _ttl = Math.Max(0, _ttl - _ttlDecreaseStep);
            else _ttl = (int) (_ttl + _maxTtlAdaptStepMs * (w.Sum / (float) w.Count));
#if TRACE_MEM_CACHE_LOST
            TraceOn.MemCacheLost(nameof(DefaultAllocationPooler),
                $"size={_size}B lost_ratio={w.Sum / (float) w.Count} ttl={_ttl}");
#endif
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReturnToPool(ref AllocationPool pool, Header alloc)
    {
        pool.Allocations.Add(new PoolItem(alloc.FreeablePointer, alloc.AllocSize, Environment.TickCount64));
    }
}