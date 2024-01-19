namespace NullGC.Collections;

public interface IValueEnumerableSorter<TElement> : IDisposable where TElement : unmanaged
{
    void SetAllocatorProviderId(int allocatorProviderId);
    unsafe void ComputeKeys(TElement* elements, int count);
    unsafe void ComputeKeys(Ptr<TElement>* elements, int count);
    int CompareAnyKeys(int index1, int index2);
}