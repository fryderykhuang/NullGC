namespace NullGC.Allocators;

public interface IAllocatorCacheable
{
    /// <summary>
    /// Free all allocations out there or cached to the underlying native allocator. 
    /// </summary>
    /// <remarks>
    /// If there are any un-freed allocations lingering there, it will become invalid, which is dangerous.  
    /// </remarks>
    void ClearCachedMemory();
}