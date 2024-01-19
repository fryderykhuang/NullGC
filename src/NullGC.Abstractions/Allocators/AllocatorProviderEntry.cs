namespace NullGC.Allocators;

public readonly struct AllocatorProviderEntry
{
    public readonly IAllocatorProvider Provider;
    public readonly int Id;
    public readonly bool Scoped;

    public AllocatorProviderEntry(IAllocatorProvider provider, int id, bool scoped)
    {
        Provider = provider;
        Id = id;
        Scoped = scoped;
    }

    public static AllocatorProviderEntry CreateScoped(IAllocatorProvider provider, int id) => new(provider, id, true);
    public static AllocatorProviderEntry CreateUnscoped(IAllocatorProvider provider, int id) => new(provider, id, false);
}