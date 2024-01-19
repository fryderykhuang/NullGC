using NullGC.Allocators;
using NullGC.Allocators.Extensions;

namespace NullGC.TestCommons;

public static class AllocatorContextInitializer
{
    public static void SetupDefaultAllocationContext(out IMemoryAllocationTrackable allocTracker,
        out IMemoryAllocationTrackable nativeTracker)
    {
        var nt = new DefaultAllocationPooler(new DefaultAlignedNativeMemoryAllocator(), 1000);
        nativeTracker = nt;
        var at = new AllocatorPool<ArenaAllocator>(p => new ArenaAllocator(p, p, nt));
        allocTracker = at;
        AllocatorContext.SetAllocatorProvider(at, (int) AllocatorTypes.Default, true);
    }

    public static void SetupDefaultUnscopedAllocationContext(out IMemoryAllocationTrackable allocTracker,
        out IMemoryAllocationTrackable nativeTracker)
    {
        var nt = new DefaultAllocationPooler(new DefaultAlignedNativeMemoryAllocator(), 1000);
        AllocatorContext.SetAllocatorProvider(nt, (int) AllocatorTypes.Default, false);
        allocTracker = nativeTracker = nt;
    }
    
    public static void SetupDefaultUncachedUnscopedAllocationContext(out IMemoryAllocationTrackable allocTracker,
        out IMemoryAllocationTrackable nativeTracker)
    {
        var nt = new DefaultAlignedNativeMemoryAllocator();
        AllocatorContext.SetAllocatorProvider(nt, (int) AllocatorTypes.Default, false);
        allocTracker = nativeTracker = null;
    }
}