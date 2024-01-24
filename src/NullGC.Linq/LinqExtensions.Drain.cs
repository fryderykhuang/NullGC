using System.Numerics;
using System.Runtime.CompilerServices;
using NullGC.Collections;
using NullGC.Linq.Enumerators;

namespace NullGC.Linq;

public static partial class LinqExtensions
{
    #region Drain
    internal static class DummyLoad<T>
    {
        public static T? Dummy;
    }

    public static void Drain<T>(this IEnumerable<T> src)
    {
        using var e = src.GetEnumerator();
        while (e.MoveNext()) DummyLoad<T>.Dummy = e.Current;
    }

    public static void Drain<T, TEnumerator>(this LinqFixedRefEnumerable<T, TEnumerator> src)
        where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed
    {
        using var e = src.GetEnumerator();
        while (e.MoveNext()) DummyLoad<T>.Dummy = e.Current;
    }

    public static void Drain<T, TEnumerator>(this LinqRefEnumerable<T, TEnumerator> src)
        where TEnumerator : struct, ILinqRefEnumerator<T>
    {
        using var e = src.GetEnumerator();
        while (e.MoveNext()) DummyLoad<T>.Dummy = e.Current;
    }

    public static void Drain<T, TEnumerator>(this LinqValueEnumerable<T, TEnumerator> src)
        where TEnumerator : struct, ILinqValueEnumerator<T>
    {
        using var e = src.GetEnumerator();
        while (e.MoveNext()) DummyLoad<T>.Dummy = e.Current;
    }

    public static void Drain<T, TEnumerator>(this LinqPtrEnumerable<T, TEnumerator> src)
        where TEnumerator : struct, ILinqRefEnumerator<T>, IAddressFixed
    {
        using var e = src.GetEnumerator();
        while (e.MoveNext())
            unsafe
            {
                DummyLoad<T>.Dummy = *e.CurrentPtr;
            }
    }

    #endregion
}