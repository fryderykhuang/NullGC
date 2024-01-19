namespace NullGC.Allocators;

public interface IAllocatorProvider
{
    IMemoryAllocator GetAllocator();
}