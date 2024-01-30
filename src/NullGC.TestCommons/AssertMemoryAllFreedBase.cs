using NullGC.Allocators;
using NullGC.Collections.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace NullGC.TestCommons;

[Collection("AllocatorContext")]
public abstract class AssertMemoryAllFreedBase : TestBase, IDisposable, IClassFixture<DefaultAllocatorContextFixture>
{
    private readonly bool _scoped;
    private readonly IMemoryAllocationTrackable? _memTrackable;
    private readonly IMemoryAllocationTrackable? _allocTrackable;

    protected IMemoryAllocationTrackable? AllocTracker => _allocTrackable;

    protected AssertMemoryAllFreedBase(ITestOutputHelper logger, bool scoped, bool uncached = false) : base(logger)
    {
        AllocatorContext.ClearProvidersAndAllocations();

        _scoped = scoped;
        if (scoped)
            AllocatorContextInitializer.SetupDefaultAllocationContext(out _allocTrackable, out _memTrackable);
        else if (!uncached)
            AllocatorContextInitializer.SetupDefaultUnscopedAllocationContext(out _allocTrackable, out _memTrackable);
        else
            AllocatorContextInitializer.SetupDefaultUncachedUnscopedAllocationContext(out _allocTrackable,
                out _memTrackable);
    }

    public override void Dispose()
    {
        if (_allocTrackable is IAllocatorCacheable c1) c1.ClearCachedMemory();
        if (_allocTrackable is not null)
            Assert.True(_allocTrackable.ClientIsAllFreed);
        if (_allocTrackable is IAllocatorCacheable c2) c2.ClearCachedMemory();
        if (_scoped) 
            if (_memTrackable is not null) Assert.True(_memTrackable.IsAllFreed);
        AllocatorContext.ClearProvidersAndAllocations();
    }
}