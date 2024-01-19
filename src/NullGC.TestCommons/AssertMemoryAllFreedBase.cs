using NullGC.Allocators;
using Xunit;

namespace NullGC.TestCommons;

[Collection("AllocatorContext")]
public abstract class AssertMemoryAllFreedBase : IDisposable, IClassFixture<DefaultAllocatorContextFixture>
{
    private readonly bool _scoped;
    private readonly IMemoryAllocationTrackable _memTrackable;
    private readonly IMemoryAllocationTrackable _allocTrackable;

    protected IMemoryAllocationTrackable AllocTracker => _allocTrackable;

    protected AssertMemoryAllFreedBase(bool scoped, bool uncached = false)
    {
        AllocatorContext.ClearProvidersAndAllocations();

        _scoped = scoped;
        if (scoped)
            AllocatorContextInitializer.SetupDefaultAllocationContext(out _allocTrackable, out _memTrackable);
        else if (!uncached)
            AllocatorContextInitializer.SetupDefaultUnscopedAllocationContext(out _allocTrackable, out _memTrackable);
        else
            AllocatorContextInitializer.SetupDefaultUncachedUnscopedAllocationContext(out _allocTrackable, out _memTrackable);
    }

    public virtual void Dispose()
    {
        if (_allocTrackable is IAllocatorCacheable c1) c1.ClearCachedMemory();
        if (_allocTrackable is not null)
            Assert.True(_allocTrackable.ClientIsAllFreed);
        if (_allocTrackable is IAllocatorCacheable c2) c2.ClearCachedMemory();
        if (_scoped)
            Assert.True(_memTrackable.IsAllFreed);
        AllocatorContext.ClearProvidersAndAllocations();
    }
}