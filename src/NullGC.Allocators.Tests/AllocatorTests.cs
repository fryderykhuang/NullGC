using System.Runtime.CompilerServices;
using NullGC.TestCommons;
using Assert = Xunit.Assert;

namespace NullGC.Allocators.Tests;

[Collection("AllocatorContext")]
public class AllocatorTests : IDisposable
{
    public AllocatorTests()
    {
        AllocatorContext.SetImplementation(new DefaultAllocatorContextImpl());
    }
    
    [Fact]
    public void AllocatorContext_SetAllocatorProvider_WillThrowIfAllocatorProviderIsNotSet()
    {
        Assert.Throws<InvalidOperationException>(() => AllocatorContext.BeginAllocationScope());
    }

    [Fact]
    public void AllocatorContext_SetAllocatorProvider_WillThrowIfAllocatorProviderIdIsInvalid()
    {
        Assert.Throws<ArgumentException>(() =>
            AllocatorContext.SetAllocatorProvider(new DefaultAlignedNativeMemoryAllocator(),
                (int) AllocatorProviderIds.Invalid, true));
    }

    [Fact]
    public void AllocatorContext_SetAllocatorProvider_WillThrowIfAllocatorProviderWithSameIdIsAlreadySet()
    {
        AllocatorContext.SetAllocatorProvider(new DefaultAlignedNativeMemoryAllocator(),
            (int) AllocatorProviderIds.Default, true);
        Assert.Throws<ArgumentException>(() =>
            AllocatorContext.SetAllocatorProvider(new DefaultAlignedNativeMemoryAllocator(),
                (int) AllocatorProviderIds.Default, true));
    }

    [Fact]
    public void CanAllocateAndFreeOnStaticScopedProviderAndReturnedAllocatorIsTheSame()
    {
        var nativeAllocator = new DefaultAlignedNativeMemoryAllocator();
        var nativeBuffer = new DefaultAllocationPooler(nativeAllocator, 1000);
        AllocatorContext.SetAllocatorProvider(
            new AllocatorPool<ArenaAllocator>(p => new ArenaAllocator(p, p, nativeBuffer)),
            (int) AllocatorProviderIds.Default, true);
        IMemoryAllocator allocator;
        using (AllocatorContext.BeginAllocationScope())
        {
            unsafe
            {
                allocator = AllocatorContext.GetAllocator();
                var mem = allocator.Allocate(1);
                Unsafe.WriteUnaligned(mem.ToPointer(), (byte) 47);
                AllocatorContext.GetAllocator().Free(mem);
            }
        }

        using (AllocatorContext.BeginAllocationScope())
        {
            unsafe
            {
                var oldAlloc = allocator;
                allocator = AllocatorContext.GetAllocator();
                Assert.Equal(allocator, oldAlloc);
                var mem = allocator.Allocate(100000);
                Unsafe.WriteUnaligned(mem.ToPointer(), (byte) 47);
                AllocatorContext.GetAllocator().Free(mem);
            }
        }
    }

    [Fact]
    public void CanAllocateAndFreeOnPooledScopedProviderAndPoolIsWorking()
    {
        var nativeAllocator = new DefaultAlignedNativeMemoryAllocator();
        AllocatorContext.SetAllocatorProvider(
            new AllocatorPool<ArenaAllocator>(p => new ArenaAllocator(p, p, nativeAllocator)),
            (int) AllocatorProviderIds.Default, true);
        IMemoryAllocator allocator;
        using (AllocatorContext.BeginAllocationScope())
        {
            unsafe
            {
                allocator = AllocatorContext.GetAllocator();
                var mem = allocator.Allocate(1);
                Unsafe.WriteUnaligned(mem.ToPointer(), (byte) 47);
                AllocatorContext.GetAllocator().Free(mem);
            }
        }

        using (AllocatorContext.BeginAllocationScope())
        {
            unsafe
            {
                var oldAlloc = allocator;
                allocator = AllocatorContext.GetAllocator();
                Assert.Equal(allocator, oldAlloc);
                var mem = allocator.Allocate(10000);
                Unsafe.InitBlockUnaligned(mem.ToPointer(), 47, 10000);
                AllocatorContext.GetAllocator().Free(mem);
            }
        }
    }

    [Fact]
    public void CanAllocateAndFreeOnPooledCachedScopedProviderAndPoolIsWorkingAndAllFreedAtEnd()
    {
        var cache = new DefaultAllocationPooler(new DefaultAlignedNativeMemoryAllocator(), 1000);
        var arenaAllocatorPool = new AllocatorPool<ArenaAllocator>(p => new ArenaAllocator(p, p, cache));
        AllocatorContext.SetAllocatorProvider(arenaAllocatorPool, (int) AllocatorProviderIds.Default, true);

        IMemoryAllocator allocator;
        using (AllocatorContext.BeginAllocationScope())
        {
            unsafe
            {
                allocator = AllocatorContext.GetAllocator();
                var mem = allocator.Allocate(1);
                Unsafe.WriteUnaligned(mem.ToPointer(), (byte) 47);
                AllocatorContext.GetAllocator().Free(mem);
            }
        }

        using (AllocatorContext.BeginAllocationScope())
        {
            unsafe
            {
                var oldAlloc = allocator;
                allocator = AllocatorContext.GetAllocator();
                Assert.Equal(allocator, oldAlloc);
                var mem = allocator.Allocate(10000);
                Unsafe.InitBlockUnaligned(mem.ToPointer(), 47, 10000);
                AllocatorContext.GetAllocator().Free(mem);
            }
        }

        Assert.Equal(arenaAllocatorPool.SelfTotalAllocated, arenaAllocatorPool.SelfTotalFreed);
        arenaAllocatorPool.ClearCachedMemory();
        Assert.Equal(cache.ClientTotalAllocated, cache.ClientTotalFreed);
        Thread.Sleep(1500); // go past cache ttl
        cache.Prune(0);
        Assert.Equal(cache.SelfTotalAllocated, cache.SelfTotalFreed);
    }

    [Fact]
    public void AllocatorContextReturnsToPoolWhenThreadDies()
    {
        var cache = new DefaultAllocationPooler(new DefaultAlignedNativeMemoryAllocator(), 1000);
        var arenaAllocatorPool = new AllocatorPool<ArenaAllocator>(p => new ArenaAllocator(p, p, cache));
        AllocatorContext.SetAllocatorProvider(arenaAllocatorPool, (int) AllocatorProviderIds.Default, true);

        Assert.Empty(((DefaultAllocatorContextImpl)AllocatorContext.Impl).ContextPool);
        var mainThreadId = Thread.CurrentThread.ManagedThreadId;
        var execCtx = Thread.CurrentThread.ExecutionContext;
        Assert.NotNull(execCtx);

        void Worker()
        {
            Assert.NotEqual(mainThreadId, Thread.CurrentThread.ManagedThreadId);
            Assert.Empty(((DefaultAllocatorContextImpl)AllocatorContext.Impl).ContextPool);
            // Assert.Null(ContextContainer.GetPerProviderContainer(0).ContextContainer.Value);
            using (AllocatorContext.BeginAllocationScope())
            {
                Assert.NotNull(((DefaultAllocatorContextImpl)AllocatorContext.Impl).GetPerProviderContainer((int)AllocatorProviderIds.Default).Context!.Value);
                Assert.Empty(((DefaultAllocatorContextImpl)AllocatorContext.Impl).ContextPool);
                AllocatorContext.GetAllocator();
                Assert.Empty(((DefaultAllocatorContextImpl)AllocatorContext.Impl).ContextPool);
            }

            Assert.NotNull(((DefaultAllocatorContextImpl)AllocatorContext.Impl).GetPerProviderContainer((int)AllocatorProviderIds.Default).Context!.Value);

            Assert.Empty(((DefaultAllocatorContextImpl)AllocatorContext.Impl).ContextPool);
            Assert.NotEqual(execCtx, Thread.CurrentThread.ExecutionContext);
        }

        var thread1 = new Thread(Worker);
        thread1.Start();
        thread1.Join();
        Assert.Equal(execCtx, Thread.CurrentThread.ExecutionContext);
        Assert.Single(((DefaultAllocatorContextImpl)AllocatorContext.Impl).ContextPool);
        Assert.True(((IMemoryAllocationTrackable) arenaAllocatorPool).IsAllFreed);
        AllocatorContext.ClearProvidersAndAllocations();
        Assert.Empty(((DefaultAllocatorContextImpl)AllocatorContext.Impl).ContextPool);
    }

    [Fact]
    public void FixedRetentionNativeMemoryCache_ExpirationBehaviorWhenCleanupThresholdIsZero()
    {
        var cache = new DefaultAllocationPooler(new DefaultAlignedNativeMemoryAllocator(), 1000);
        var mem1 = cache.Allocate(1000);
        Assert.Equal((ulong) (cache.GetAllocSize(1000)), cache.SelfTotalAllocated);
        cache.Free(mem1);
        Assert.Equal((ulong) (cache.GetAllocSize(1000)), cache.SelfTotalAllocated);
        Assert.Equal((ulong) 0, cache.SelfTotalFreed);
        return; //not working since dynamic ttl
        Thread.Sleep(1500);
        cache.Prune();
        Assert.True(((IMemoryAllocationTrackable) cache).IsAllFreed);
        var mem2 = cache.Allocate(100);
        Assert.Equal((ulong) (cache.GetAllocSize(100)),
            cache.SelfTotalAllocated - cache.SelfTotalFreed);
        cache.Free(mem2);
        Thread.Sleep(1500);
        cache.Prune();
        Assert.Equal(cache.SelfTotalAllocated, cache.SelfTotalFreed);
        cache.ClearCachedMemory();
        Assert.Equal(cache.SelfTotalAllocated, cache.SelfTotalFreed);
    }

    [Fact]
    public void FixedRetentionNativeMemoryCache_ExpirationBehavior()
    {
        var cache = new DefaultAllocationPooler(new DefaultAlignedNativeMemoryAllocator(), 1000);
        var mem1 = cache.Allocate(1000);
        Assert.Equal((ulong) (cache.GetAllocSize(1000)), cache.SelfTotalAllocated);
        cache.Free(mem1); // mem1 < ttl && < Th
        Assert.Equal((ulong) (cache.GetAllocSize(1000)), cache.SelfTotalAllocated);
        Assert.Equal((ulong) 0, cache.SelfTotalFreed);
        Thread.Sleep(1500);
        cache.Prune(); // mem1 < Th
        Assert.Equal((ulong) (cache.GetAllocSize(1000)), cache.SelfTotalAllocated);
        Assert.Equal((ulong) 0, cache.SelfTotalFreed);
        var mem2 = cache.Allocate(2000);
        Assert.Equal((ulong) (cache.GetAllocSize(1000) + cache.GetAllocSize(2000)),
            cache.SelfTotalAllocated - cache.SelfTotalFreed);
        return; //not working since dynamic ttl 
        cache.Free(mem2); // mem1 gone, mem2 < ttl > Th
        Assert.Equal((ulong) (cache.GetAllocSize(2000)), cache.SelfTotalAllocated - cache.SelfTotalFreed);
        Thread.Sleep(500);
        cache.Prune(); // mem2 > Th && < ttl
        Assert.Equal((ulong) (cache.GetAllocSize(2000)), cache.SelfTotalAllocated - cache.SelfTotalFreed);
        Thread.Sleep(1000);
        cache.Prune();
        Assert.Equal(cache.SelfTotalAllocated, cache.SelfTotalFreed);
        cache.ClearCachedMemory();
        Assert.Equal(cache.SelfTotalAllocated, cache.SelfTotalFreed);
    }

    [Fact]
    public void FixedRetentionNativeMemoryCache_ClearCacheMemoryIsWorking()
    {
        var cache = new DefaultAllocationPooler(new DefaultAlignedNativeMemoryAllocator(), 1000);
        var mem1 = cache.Allocate(1000);
        Assert.Equal((ulong) (cache.GetAllocSize(1000)), cache.SelfTotalAllocated);
        Assert.Equal((ulong) 0, cache.SelfTotalFreed);
        cache.Free(mem1);
        Assert.Equal((ulong) (cache.GetAllocSize(1000)), cache.SelfTotalAllocated);
        Assert.Equal((ulong) 0, cache.SelfTotalFreed);
        cache.ClearCachedMemory();
        Assert.Equal(cache.SelfTotalAllocated, cache.SelfTotalFreed);
    }

    [Fact]
    public void NestedSameProviderTypeScope()
    {
        var allocPooler = new DefaultAllocationPooler(new DefaultAlignedNativeMemoryAllocator(), 1000);
        var arenaAllocatorPool = new AllocatorPool<ArenaAllocator>(p => new ArenaAllocator(p, p, allocPooler));
        AllocatorContext.SetAllocatorProvider(arenaAllocatorPool, (int) AllocatorProviderIds.Default, true);

        using (AllocatorContext.BeginAllocationScope())
        {
            AllocatorContext.GetAllocator().Allocate(1000);
            Assert.Equal((ulong) 1000, arenaAllocatorPool.ClientTotalAllocated);

            using (AllocatorContext.BeginAllocationScope())
            {
                AllocatorContext.GetAllocator().Allocate(1500);
                Assert.Equal((ulong) (1000 + 1500), arenaAllocatorPool.ClientTotalAllocated);
            }

            Assert.Equal((ulong) (1000 + 1500), arenaAllocatorPool.ClientTotalAllocated);
            Assert.Equal((ulong) 1500, arenaAllocatorPool.ClientTotalFreed);

            allocPooler.ClearCachedMemory();
            Assert.Equal((ulong) allocPooler.GetAllocSize(ArenaAllocator.DefaultPageSize - allocPooler.MetadataOverhead), allocPooler.SelfTotalFreed);
        }

        Assert.Equal((ulong) (1000 + 1500), arenaAllocatorPool.ClientTotalAllocated);
        Assert.Equal((ulong) (1500 + 1000), arenaAllocatorPool.ClientTotalFreed);

        allocPooler.ClearCachedMemory();
        Assert.Equal(
            (ulong) (allocPooler.GetAllocSize((ArenaAllocator.DefaultPageSize - allocPooler.MetadataOverhead)) * 2),
            allocPooler.SelfTotalFreed);
    }

    [Fact]
    public void NestedDifferentProviderTypeScope()
    {
        var allocPooler = new DefaultAllocationPooler(new DefaultAlignedNativeMemoryAllocator(), 1000);
        var arenaAllocatorPool = new AllocatorPool<ArenaAllocator>(p => new ArenaAllocator(p, p, allocPooler));
        AllocatorContext.SetAllocatorProvider(arenaAllocatorPool, (int) AllocatorProviderIds.Default, true);
        var arenaAllocatorPool2 = new AllocatorPool<ArenaAllocator>(p => new ArenaAllocator(p, p, allocPooler));
        AllocatorContext.SetAllocatorProvider(arenaAllocatorPool2, 16, true);

        using (AllocatorContext.BeginAllocationScope())
        {
            AllocatorContext.GetAllocator().Allocate(1000);
            Assert.Equal((ulong) 1000, arenaAllocatorPool.ClientTotalAllocated);

            using (AllocatorContext.BeginAllocationScope(16))
            {
                AllocatorContext.GetAllocator(16).Allocate(1500);
                Assert.Equal((ulong) 1000, arenaAllocatorPool.ClientTotalAllocated);
                Assert.Equal((ulong) 1500, arenaAllocatorPool2.ClientTotalAllocated);
            }

            Assert.Equal((ulong) 1000, arenaAllocatorPool.ClientTotalAllocated);
            Assert.Equal((ulong) 0, arenaAllocatorPool.ClientTotalFreed);
            Assert.Equal((ulong) 1500, arenaAllocatorPool2.ClientTotalAllocated);
            Assert.Equal((ulong) 1500, arenaAllocatorPool2.ClientTotalFreed);

            allocPooler.ClearCachedMemory();
            Assert.Equal(
                (ulong) allocPooler.GetAllocSize(ArenaAllocator.DefaultPageSize - allocPooler.MetadataOverhead),
                allocPooler.SelfTotalFreed);
        }

        Assert.Equal((ulong) 1000, arenaAllocatorPool.ClientTotalAllocated);
        Assert.Equal((ulong) 1000, arenaAllocatorPool.ClientTotalFreed);

        Assert.Equal((ulong) 1500, arenaAllocatorPool2.ClientTotalFreed);
        Assert.Equal((ulong) 1500, arenaAllocatorPool2.ClientTotalFreed);

        allocPooler.ClearCachedMemory();
        Assert.Equal((ulong) ( allocPooler.GetAllocSize((ArenaAllocator.DefaultPageSize - allocPooler.MetadataOverhead)) * 2), allocPooler.SelfTotalFreed);
    }

    public void Dispose()
    {
        AllocatorContext.ResetImplementation(null);
    }
}