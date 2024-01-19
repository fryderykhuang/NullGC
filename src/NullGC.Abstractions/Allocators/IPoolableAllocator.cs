namespace NullGC.Allocators;

public interface IPoolableAllocator
{
    void ReturnToPool();
}