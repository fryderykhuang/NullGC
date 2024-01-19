using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NullGC.Linq;

public static partial class LinqExtensions
{
    #region Average

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<double> Average<TEnumerator>(this LinqRefEnumerable<int, TEnumerator> source)
        where TEnumerator : struct, ILinqRefEnumerator<int>
    {
        return Average<int, LinqRefEnumerable<int, TEnumerator>, TEnumerator, long, double>(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<double> Average<TEnumerator>(this LinqPtrEnumerable<int, TEnumerator> source)
        where TEnumerator : struct, ILinqRefEnumerator<int>, IItemAddressFixed
    {
        return Average<int, LinqPtrEnumerable<int, TEnumerator>, TEnumerator, long, double>(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<double> Average<TEnumerator>(this LinqValueEnumerable<int, TEnumerator> source)
        where TEnumerator : struct, ILinqValueEnumerator<int>
    {
        return Average<int, LinqValueEnumerable<int, TEnumerator>, TEnumerator, long, double>(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<double> Average<TEnumerator>(this LinqRefEnumerable<long, TEnumerator> source)
        where TEnumerator : struct, ILinqRefEnumerator<long>
    {
        return Average<long, LinqRefEnumerable<long, TEnumerator>, TEnumerator, long, double>(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<double> Average<TEnumerator>(this LinqValueEnumerable<long, TEnumerator> source)
        where TEnumerator : struct, ILinqValueEnumerator<long>
    {
        return Average<long, LinqValueEnumerable<long, TEnumerator>, TEnumerator, long, double>(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<double> Average<TEnumerator>(this LinqRefEnumerable<uint, TEnumerator> source)
        where TEnumerator : struct, ILinqRefEnumerator<uint>
    {
        return Average<uint, LinqRefEnumerable<uint, TEnumerator>, TEnumerator, ulong, double>(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<double> Average<TEnumerator>(this LinqValueEnumerable<uint, TEnumerator> source)
        where TEnumerator : struct, ILinqValueEnumerator<uint>
    {
        return Average<uint, LinqValueEnumerable<uint, TEnumerator>, TEnumerator, ulong, double>(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<double> Average<TEnumerator>(this LinqRefEnumerable<ulong, TEnumerator> source)
        where TEnumerator : struct, ILinqRefEnumerator<ulong>
    {
        return Average<ulong, LinqRefEnumerable<ulong, TEnumerator>, TEnumerator, ulong, double>(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<double> Average<TEnumerator>(this LinqValueEnumerable<ulong, TEnumerator> source)
        where TEnumerator : struct, ILinqValueEnumerator<ulong>
    {
        return Average<ulong, LinqValueEnumerable<ulong, TEnumerator>, TEnumerator, ulong, double>(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<double> Average<TEnumerator>(this LinqRefEnumerable<double, TEnumerator> source)
        where TEnumerator : struct, ILinqRefEnumerator<double>
    {
        return Average<double, LinqRefEnumerable<double, TEnumerator>, TEnumerator, double, double>(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<double> Average<TEnumerator>(this LinqValueEnumerable<double, TEnumerator> source)
        where TEnumerator : struct, ILinqValueEnumerator<double>
    {
        return Average<double, LinqValueEnumerable<double, TEnumerator>, TEnumerator, double, double>(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<float> Average<TEnumerator>(this LinqRefEnumerable<float, TEnumerator> source)
        where TEnumerator : struct, ILinqRefEnumerator<float>
    {
        return Average<float, LinqRefEnumerable<float, TEnumerator>, TEnumerator, float, float>(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<float> Average<TEnumerator>(this LinqValueEnumerable<float, TEnumerator> source)
        where TEnumerator : struct, ILinqValueEnumerator<float>
    {
        return Average<float, LinqValueEnumerable<float, TEnumerator>, TEnumerator, float, float>(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Option<TResult> Average<T, TCollection, TEnumerator, TAccumulator, TResult>(this TCollection source)
        where T : struct, INumber<T>
        where TCollection : ILinqEnumerable<T, TEnumerator>
        where TEnumerator : struct, ILinqEnumerator<T>
        where TResult : struct, INumber<TResult>
        where TAccumulator : struct, INumber<TAccumulator>
    {
        using (var e = source.GetEnumerator())
        {
            var span = SpanHelper.GetReadOnlySpanOrDefault<T, TEnumerator>(e, out var success);
            if (success)
            {
                if (span.IsEmpty)
                {
                    return new Option<TResult>();
                }
                if (typeof(T) == typeof(int) && typeof(TResult) == typeof(double))
                {
                    return Unsafe.BitCast<Option<double>, Option<TResult>>(MemoryMarshal.Cast<T, int>(span)
                            .Average());
                }
                else
                {
                    return span.Average<T, TAccumulator, TResult>();
                }
            }
            else if (typeof(TEnumerator).IsAssignableTo(typeof(ILinqRefEnumerator<T>)))
            {
                if (!e.MoveNext()) return new Option<TResult>();
                TAccumulator sum = TAccumulator.CreateChecked(((ILinqRefEnumerator<T>) e).Current);
                long count = 1;
                while (e.MoveNext())
                {
                    checked
                    {
                        sum += TAccumulator.CreateChecked(((ILinqRefEnumerator<T>) e).Current);
                    }

                    count++;
                }

                return new Option<TResult>(TResult.CreateChecked(sum) / TResult.CreateChecked(count));
            }
            else if (typeof(TEnumerator).IsAssignableTo(typeof(ILinqValueEnumerator<T>)))
            {
                if (!e.MoveNext()) return new Option<TResult>();
                TAccumulator sum = TAccumulator.CreateChecked(((ILinqValueEnumerator<T>) e).Current);
                long count = 1;
                while (e.MoveNext())
                {
                    checked
                    {
                        sum += TAccumulator.CreateChecked(((ILinqValueEnumerator<T>) e).Current);
                    }

                    count++;
                }

                return new Option<TResult>(TResult.CreateChecked(sum) / TResult.CreateChecked(count));
            }
            else
            {
                return CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException<Option<TResult>>();
            }
        }
    }

    #endregion
}