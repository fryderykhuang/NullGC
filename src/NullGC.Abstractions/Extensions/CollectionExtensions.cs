using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NullGC.Collections;

namespace NullGC.Extensions;

public static class CollectionExtensions
{
    public static ref TValue GetValueRefOrNullRef<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key) where TKey : notnull
    {
        return ref CollectionsMarshal.GetValueRefOrNullRef(dict, key);
    }
    
    public static ref TValue GetValueRefOrNullRef<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, out bool exists) where TKey : notnull
    {
        ref var ret = ref CollectionsMarshal.GetValueRefOrNullRef(dict, key);
        exists = !Unsafe.IsNullRef(ref ret);
        return ref ret;
    }
    
    public static ref TValue? GetValueRefOrAddDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, out bool exist) where TKey : notnull
    {
        return ref CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out exist);
    }
}