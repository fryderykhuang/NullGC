namespace NullGC.Allocators;

/// <summary>
/// Expose stats on memory allocations come from the implementer.
/// </summary>
public interface IMemoryAllocationTrackable
{
    public ulong SelfTotalAllocated { get; }
    public ulong SelfTotalFreed { get; }
    public bool IsAllFreed => SelfTotalAllocated == SelfTotalFreed;
    
    public ulong ClientTotalAllocated { get; }
    public ulong ClientTotalFreed { get; }
    
    public bool ClientIsAllFreed => ClientTotalAllocated == ClientTotalFreed;
}