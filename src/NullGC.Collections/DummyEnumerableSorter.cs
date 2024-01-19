namespace NullGC.Collections;

public struct DummyEnumerableSorter<TElement> : IValueEnumerableSorter<TElement> where TElement : unmanaged
{
    public void SetAllocatorProviderId(int allocatorProviderId)
    {
    }

    public unsafe void ComputeKeys(TElement* elements, int count)
    {
    }

    public unsafe void ComputeKeys(Ptr<TElement>* elements, int count)
    {
    }
    
    public int CompareAnyKeys(int index1, int index2)
    {
        return 0;
    }

    public void Dispose()
    {
    }
}