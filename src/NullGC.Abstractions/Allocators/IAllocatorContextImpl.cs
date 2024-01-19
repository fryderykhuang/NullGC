namespace NullGC.Allocators;

public interface IAllocatorContextImpl
{
    void FreeAllocations(int allocatorProviderId);
    void SetAllocatorProvider(IAllocatorProvider provider, int allocatorProviderId, bool scoped);
    void ClearProvidersAndAllocations();
    IMemoryAllocator GetAllocator(int allocatorProviderId = (int) AllocatorProviderIds.Default);
    IDisposable BeginAllocationScope(int allocatorProviderId = (int) AllocatorProviderIds.Default);
    void FinalizeConfiguration();
}