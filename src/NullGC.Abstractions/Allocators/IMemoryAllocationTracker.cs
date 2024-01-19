namespace NullGC.Allocators;

public interface IMemoryAllocationTracker
{
    public void ClientAllocate(ulong bytes);
    public void ClientFree(ulong bytes);
}