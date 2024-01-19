namespace NullGC.Allocators;

public enum AllocatorProviderIds
{
    Invalid = 0,
    Default = 1,
    ScopedUserMin = 16,
    DefaultUnscoped = -1,
    DefaultUncachedUnscoped = -2,
    UnscopedUserMax = -16,
}