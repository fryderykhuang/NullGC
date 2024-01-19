namespace NullGC.Allocators.Extensions;

public static class DefaultAllocatorContextImplExtensions
{
    public static IAllocatorContextImpl ConfigureDefault(this IAllocatorContextImpl ac,
        int defaultArenaAllocationCacheRetentionTimeMs = 10000)
    {
        return ConfigureDefault(ac, out _, out _, defaultArenaAllocationCacheRetentionTimeMs);
    }

    public static IAllocatorContextImpl ConfigureDefaultUnscoped(this IAllocatorContextImpl ac,
        int cacheTtlMs = 10000, int cacheLostObserveWindowSize = 100, UIntPtr cleanupTh = 8 * 1024 * 1024)
    {
        return ConfigureDefaultUnscoped(ac, out _, out _, cleanupTh: cleanupTh, cacheTtl: cacheTtlMs,
            cacheLostObserveWindowSize: cacheLostObserveWindowSize);
    }

    public static IAllocatorContextImpl ConfigureDefault(this IAllocatorContextImpl ac,
        out IMemoryAllocationTrackable frontStats, out IMemoryAllocationTrackable? cacheAllocStats,
        int defaultTtlMs = 10000, int cacheLostObserveWindowSize = 100, nuint cleanupTh = 8 * 1024 * 1024)
    {
        var memPoolerUnscoped = new DefaultAllocationPooler(new DefaultAlignedNativeMemoryAllocator(),
            defaultTtlMs);
        ac.SetAllocatorProvider(memPoolerUnscoped, (int) AllocatorTypes.DefaultUnscoped, false);
        var cache = new DefaultAllocationPooler(new DefaultAlignedNativeMemoryAllocator(),
            defaultTtlMs, cleanupTh, cacheLostObserveWindowSize: cacheLostObserveWindowSize);
        cacheAllocStats = cache;
        var allocatorPooler = new AllocatorPool<ArenaAllocator>(p => new ArenaAllocator(p, p, cache));
        frontStats = allocatorPooler;
        ac.SetAllocatorProvider(allocatorPooler, (int) AllocatorTypes.Default, true);
        return ac;
    }

    public static IAllocatorContextImpl ConfigureDefaultUnscoped(this IAllocatorContextImpl ac,
        out IMemoryAllocationTrackable frontStats, out IMemoryAllocationTrackable? cacheAllocStats,
        int cacheTtl = 10000, int cacheLostObserveWindowSize = 100, nuint cleanupTh = 8 * 1024 * 1024)
    {
        var uncached = new DefaultAlignedNativeMemoryAllocator();
        var memPoolerUnscoped =
            new DefaultAllocationPooler(uncached, cleanupThresholdBytes: cleanupTh, ttlMs: cacheTtl,
                cacheLostObserveWindowSize: cacheLostObserveWindowSize);
        ac.SetAllocatorProvider(memPoolerUnscoped, (int) AllocatorTypes.DefaultUnscoped, false);
        var cache = new DefaultAllocationPooler(new DefaultAlignedNativeMemoryAllocator(), cacheTtl);
        cacheAllocStats = null;
        ac.SetAllocatorProvider(memPoolerUnscoped, (int) AllocatorTypes.Default, false);
        frontStats = cache;
        return ac;
    }
    
    public static IAllocatorContextImpl ConfigureDefaultUncachedUnscoped(this IAllocatorContextImpl ac)
    {
        var uncached = new DefaultAlignedNativeMemoryAllocator();
        ac.SetAllocatorProvider(uncached, (int) AllocatorTypes.DefaultUnscoped, false);
        ac.SetAllocatorProvider(uncached, (int) AllocatorTypes.Default, false);
        return ac;
    }
}