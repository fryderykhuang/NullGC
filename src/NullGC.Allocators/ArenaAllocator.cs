using System.Diagnostics;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using NullGC.Collections;
using ThrowHelper = NullGC.Diagnostics.ThrowHelper;

namespace NullGC.Allocators;

// Inspired by https://github.com/Enichan/Arenas
public sealed class ArenaAllocator : IMemoryAllocator, IDisposable, IMemoryAllocationTrackable, IPoolableAllocator
{
    public const int DefaultPageSize = 4096;
    private static int _nextId;
    private readonly int _id = GetNextArenaId();
    private readonly IMemoryAllocator _innerAllocator;
    private readonly uint _pageSize;
    private readonly uint _pageSizeMinusOverhead;
    private readonly IAllocatorPool? _pool;
    private readonly IMemoryAllocationTracker? _tracker;


    private ValueDictionary<nuint, FreeList>
        _freeList = new(Environment.ProcessorCount, (int) AllocatorTypes.DefaultUncachedUnscoped);

    private ValueList<Page> _pages = new(Environment.ProcessorCount,
        (int) AllocatorTypes.DefaultUncachedUnscoped);

    private ulong _totalAllocated;
    private ulong _totalFreed;
    private ulong _clientAllocated;
    private ulong _clientFreed;
    private static readonly uint Overhead;

    static ArenaAllocator()
    {
        unsafe
        {
            Overhead = MemoryMath.Ceiling((uint) sizeof(Header), MemoryConstants.DefaultAlignment);
        }
    }
    
    public ArenaAllocator(IAllocatorPool? pool = null, IMemoryAllocationTracker? tracker = null,
        IMemoryAllocator? innerAllocator = null, uint pageSize = DefaultPageSize)
    {
        _innerAllocator = innerAllocator ?? DefaultAlignedNativeMemoryAllocator.Default;
        _pool = pool;
        _tracker = tracker;
        _pageSize = pageSize;
        _pageSizeMinusOverhead = pageSize - (uint) _innerAllocator.MetadataOverhead;
    }

    public void Dispose()
    {
        FreeAllAllocations();
        _pages.Dispose();
        _freeList.Dispose();
        GC.SuppressFinalize(this);
    }

    public ulong SelfTotalAllocated => _totalAllocated;
    public ulong SelfTotalFreed => _totalFreed;
    public bool IsAllFreed => SelfTotalAllocated == SelfTotalFreed;
    public ulong ClientTotalAllocated => _clientAllocated;
    public ulong ClientTotalFreed => _clientFreed;
    public bool ClientIsAllFreed => ClientTotalAllocated == ClientTotalFreed;

    public UIntPtr Allocate(nuint size)
    {
        UIntPtr ptr;
        ref var freeList = ref _freeList.GetValueRefOrNullRef(size, out var exists);
        if (!exists || (ptr = freeList.Pop()) == UIntPtr.Zero)
        {
            ptr = AllocateMemory(MemoryMath.Ceiling(size + Overhead, MemoryConstants.DefaultAlignment));

            var head = new Header(_id, size, UIntPtr.Zero);
            head.WriteToPtr(ptr);
            ptr += Overhead;
        }

        _tracker?.ClientAllocate(size);
        _clientAllocated += size;
        Debug.Assert(ptr % MemoryConstants.DefaultAlignment == 0);
        return ptr;
    }

    public ReallocResult TryRealloc(UIntPtr ptr, nuint minSize, nuint maxSize = 0)
    {
        if (ptr == UIntPtr.Zero || minSize > _pageSizeMinusOverhead) return ReallocResult.NotSuccess;
        
        ref var header = ref Header.FromPtr(ptr - Overhead);
        Guard.IsEqualTo(header.ArenaId, _id);
        ref var page = ref _pages[^1];
        var nextAddr = MemoryMath.Ceiling(ptr + header.RequestedSize, MemoryConstants.DefaultAlignment);
        if (!page.IsTop(nextAddr)) return ReallocResult.NotSuccess;
        var oldAlloc = (uint) (nextAddr - (ptr - Overhead));
        page.Free(oldAlloc);
        var newAlloc = MemoryMath.Ceiling(minSize + Overhead, MemoryConstants.DefaultAlignment);
        var newAllocLimit = MemoryMath.Ceiling(maxSize + Overhead, MemoryConstants.DefaultAlignment);
        (UIntPtr newPtr, nuint actualSize) = page.AllocateAtLeast(newAlloc, newAllocLimit);
        if (ptr != UIntPtr.Zero)
        {
            var usableSize = actualSize - Overhead;
            var delta = usableSize - header.RequestedSize;
            if (delta > 0) _clientAllocated += delta;
            else _clientFreed += delta;
            return new ReallocResult(newPtr, usableSize);
        }
        else
        {
            page.Allocate(oldAlloc);
            return ReallocResult.NotSuccess;
        }
    }

    public void Free(UIntPtr ptr)
    {
        if (ptr == UIntPtr.Zero)
        {
#if DEBUG
            throw new ArgumentNullException(nameof(ptr));
#else
            return;
#endif
        }

        ref var header = ref Header.FromPtr(ptr - Overhead);
        Guard.IsEqualTo(header.ArenaId, _id);
        ref var page = ref _pages[^1];
        var nextAddr = MemoryMath.Ceiling(ptr + header.RequestedSize, MemoryConstants.DefaultAlignment);
        if (page.IsTop(nextAddr))
        {
            page.Free((uint) (nextAddr - (ptr - Overhead)));
        }
        else
        {
            ref var freeList = ref _freeList.GetValueRefOrAddDefault(header.RequestedSize, out _);
            freeList.Push(ptr);
        }

        _tracker?.ClientFree(header.RequestedSize);
        _clientFreed += header.RequestedSize;
    }

    public uint MetadataOverhead => Overhead;

    public void ReturnToPool()
    {
        FreeAllAllocations();
        _pool?.Return(this);
    }

    private static int GetNextArenaId()
    {
        return Interlocked.Increment(ref _nextId);
    }

    public void FreeAllAllocations()
    {
        _freeList.Clear();
        for (var i = 0; i < _pages.Count; ++i)
        {
            ref var p = ref _pages.GetRefUnchecked(i);
            _innerAllocator.Free(p.FreeablePtr);
            _totalFreed += p.Size;
        }

        _pages.Clear();
        _tracker?.ClientFree(_clientAllocated - _clientFreed);
    }

    private ref Page AllocatePage(nuint minSize)
    {
        nuint size;
        if (minSize > _pageSizeMinusOverhead)
            size = MemoryMath.Ceiling(minSize + Overhead + _innerAllocator.MetadataOverhead, _pageSize) -
                   _innerAllocator.MetadataOverhead;
        else
            size = _pageSizeMinusOverhead;

        ref var newPage = ref _pages.AddAndReturnsRef();
        var ptr = _innerAllocator.Allocate(size);
        newPage = new Page(ptr, size);
        _totalAllocated += size;
        return ref newPage;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private UIntPtr AllocateMemory(nuint size)
    {
        EnsurePage();
        ref var page = ref _pages[^1];
        var ptr = page.Allocate(size);

        if (ptr != UIntPtr.Zero)
            return ptr;

        return AllocatePage(size).Allocate(size);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsurePage()
    {
        if (_pages.Count == 0)
            AllocatePage(_pageSizeMinusOverhead);
    }

    ~ArenaAllocator()
    {
        ThrowHelper.ObjectMustBeDisposed<ArenaAllocator>();
    }

    private struct Page
    {
        public readonly UIntPtr FreeablePtr;
        public readonly nuint Size;
        public nuint Offset;

        public Page(UIntPtr ptr, nuint size)
        {
            FreeablePtr = ptr;
            Size = size;
        }

        public UIntPtr Allocate(nuint size)
        {
            if (Offset + size > Size)
                return UIntPtr.Zero;

            var ret = FreeablePtr + Offset;
            Offset += size;
            return ret;
        }

        public (UIntPtr ptr, nuint actualSize) AllocateAtLeast(nuint size, nuint upperLimit)
        {
            var sz = Math.Min(Size - Offset, upperLimit);
            if (sz < size)
                return (UIntPtr.Zero, 0);
            size = sz;
            
            var ret = FreeablePtr + Offset;
            Offset += size;
            return (ret, size);
        }

        public void Free(uint size)
        {
            Offset -= size;
        }

        public bool IsTop(UIntPtr ptr)
        {
            return FreeablePtr + Offset == ptr;
        }

        public override string ToString()
        {
            return
                $"{nameof(FreeablePtr)}: {FreeablePtr:X}, {nameof(Size)}: {Size}, {nameof(Offset)}: {Offset}";
        }
    }

    private struct FreeList
    {
        private UIntPtr _head;

        public void Push(UIntPtr ptr)
        {
            if (_head == UIntPtr.Zero)
            {
                _head = ptr;
            }
            else
            {
                Header.SetNextFree(_head, ptr);
                _head = ptr;
            }
        }

        public UIntPtr Pop()
        {
            if (_head == UIntPtr.Zero)
                return UIntPtr.Zero;

            var ret = _head;
            var nextFree = Header.GetNextFree(_head);
            _head = nextFree;
            return ret;
        }
    }
    
    internal struct Header
    {
        public Header(int arenaId, nuint requestedSize, UIntPtr nextFree)
        {
            ArenaId = arenaId;
            RequestedSize = requestedSize;
            NextFree = nextFree;
        }

        public UIntPtr NextFree;
        public readonly nuint RequestedSize;
        public readonly int ArenaId;
        public static UIntPtr GetNextFree(UIntPtr ptr)
        {
            unsafe
            {
                return ((Header*) (ptr - (uint) sizeof(Header)))->NextFree;
            }
        }

        public static UIntPtr SetNextFree(UIntPtr ptr, UIntPtr free)
        {
            unsafe
            {
                return ((Header*) (ptr - (uint) sizeof(Header)))->NextFree = free;
            }
        }

        public void WriteToPtr(UIntPtr ptr)
        {
            unsafe
            {
                Unsafe.WriteUnaligned(ptr.ToPointer(), this);
            }
        }

        public static ref Header FromPtr(UIntPtr ptr)
        {
            unsafe
            {
                return ref Unsafe.AsRef<Header>(ptr.ToPointer());
            }
        }
    }
}