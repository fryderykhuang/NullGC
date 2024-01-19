namespace NullGC.Allocators;

public interface IAllocatorPool
{
    IMemoryAllocator Get();
    void Return(IMemoryAllocator allocator);
}