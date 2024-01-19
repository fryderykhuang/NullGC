using System.ComponentModel;

namespace NullGC.Allocators;

public enum AllocatorTypes
{
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    Invalid = 0,
    Default = 1,
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    ScopedUserMin = 16,
    DefaultUnscoped = -1,
    DefaultUncachedUnscoped = -2,
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    UnscopedUserMax = -16,
}