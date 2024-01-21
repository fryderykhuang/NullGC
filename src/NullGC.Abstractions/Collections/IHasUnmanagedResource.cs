namespace NullGC.Collections;

public interface IHasUnmanagedResource
{
    bool IsAllocated { get; }
}