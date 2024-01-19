using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NullGC.Linq;

public static class SpanExtensions
{
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
    public static TResult Sum<T, TResult>(this ReadOnlySpan<T> span)
        where T : struct, INumber<T>
        where TResult : struct, INumber<TResult>
    {
        if (typeof(T) == typeof(TResult)
            && Vector<T>.IsSupported
            && Vector.IsHardwareAccelerated
            && Vector<T>.Count > 2
            && span.Length >= Vector<T>.Count * 4)
        {
            // For cases where the vector may only contain two elements vectorization doesn't add any benefit
            // due to the expense of overflow checking. This means that architectures where Vector<T> is 128 bit,
            // such as ARM or Intel without AVX, will only vectorize spans of ints and not longs.

            if (typeof(T) == typeof(long))
            {
                return Unsafe.BitCast<long, TResult>(SumSignedIntegersVectorized(MemoryMarshal.Cast<T, long>(span)));
            }

            if (typeof(T) == typeof(int))
            {
                return Unsafe.BitCast<int, TResult>(SumSignedIntegersVectorized(MemoryMarshal.Cast<T, int>(span)));
            }
        }

        TResult sum = TResult.Zero;
        foreach (T value in span)
        {
            checked
            {
                sum += TResult.CreateChecked(value);
            }
        }

        return sum;
    }

    public static Option<double> Average(this ReadOnlySpan<int> span)
    {
        // Int32 is special-cased separately from the rest of the types as it can be vectorized:
        // with at most Int32.MaxValue values, and with each being at most Int32.MaxValue, we can't
        // overflow a long accumulator, and order of operations doesn't matter.

        if (span.IsEmpty)
        {
            return new Option<double>();
        }

        long sum = 0;
        int i = 0;

        if (Vector.IsHardwareAccelerated && span.Length >= Vector<int>.Count)
        {
            Vector<long> sums = default;
            do
            {
                Vector.Widen(new Vector<int>(span.Slice(i)), out Vector<long> low, out Vector<long> high);
                sums += low;
                sums += high;
                i += Vector<int>.Count;
            } while (i <= span.Length - Vector<int>.Count);

            sum += Vector.Sum(sums);
        }

        for (; (uint) i < (uint) span.Length; i++)
        {
            sum += span[i];
        }

        return new Option<double>((double) sum / span.Length);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<TResult> Average<TSource, TAccumulator, TResult>(this ReadOnlySpan<TSource> span)
        where TSource : struct, INumber<TSource>
        where TAccumulator : struct, INumber<TAccumulator>
        where TResult : struct, INumber<TResult>
    {
        if (span.IsEmpty)
        {
            return new Option<TResult>();
        }

        if (typeof(TSource) == typeof(int) && typeof(TResult) == typeof(double))
        {
            return Unsafe.BitCast<Option<double>, Option<TResult>>(Average(MemoryMarshal.Cast<TSource, int>(span)));
        }
        else
        {
            return new Option<TResult>(TResult.CreateChecked(Sum<TSource, TAccumulator>(span)) /
                                       TResult.CreateChecked(span.Length));
        }
    }


    // Licensed to the .NET Foundation under one or more agreements.
    // The .NET Foundation licenses this file to you under the MIT license.
    private static T SumSignedIntegersVectorized<T>(ReadOnlySpan<T> span)
        where T : struct, IBinaryInteger<T>, ISignedNumber<T>, IMinMaxValue<T>
    {
        Debug.Assert(span.Length >= Vector<T>.Count * 4);
        Debug.Assert(Vector<T>.Count > 2);
        Debug.Assert(Vector.IsHardwareAccelerated);

        ref T ptr = ref MemoryMarshal.GetReference(span);
        nuint length = (nuint) span.Length;

        // Overflow testing for vectors is based on setting the sign bit of the overflowTracking
        // vector for an element if the following are all true:
        //   - The two elements being summed have the same sign bit. If one element is positive
        //     and the other is negative then an overflow is not possible.
        //   - The sign bit of the sum is not the same as the sign bit of the previous accumulator.
        //     This indicates that the new sum wrapped around to the opposite sign.
        //
        // This is done by:
        //   overflowTracking |= (result ^ input1) & (result ^ input2);
        //
        // The general premise here is that we're doing signof(result) ^ signof(input1). This will produce
        // a sign-bit of 1 if they differ and 0 if they are the same. We do the same with
        // signof(result) ^ signof(input2), then combine both results together with a logical &.
        //
        // Thus, if we had a sign swap compared to both inputs, then signof(input1) == signof(input2) and
        // we must have overflowed.
        //
        // By bitwise or-ing the overflowTracking vector for each step we can save cycles by testing
        // the sign bits less often. If any iteration has the sign bit set in any element it indicates
        // there was an overflow.
        //
        // Note: The overflow checking in this algorithm is only correct for signed integers.
        // If support is ever added for unsigned integers then the overflow check should be:
        //   overflowTracking |= (input1 & input2) | Vector.AndNot(input1 | input2, result);

        Vector<T> accumulator = Vector<T>.Zero;

        // Build a test vector with only the sign bit set in each element.
        Vector<T> overflowTestVector = new(T.MinValue);

        // Unroll the loop to sum 4 vectors per iteration. This reduces range check
        // and overflow check frequency, allows us to eliminate move operations swapping
        // accumulators, and may have pipelining benefits.
        nuint index = 0;
        nuint limit = length - (nuint) Vector<T>.Count * 4;
        do
        {
            // Switch accumulators with each step to avoid an additional move operation
            Vector<T> data = Vector.LoadUnsafe(ref ptr, index);
            Vector<T> accumulator2 = accumulator + data;
            Vector<T> overflowTracking = (accumulator2 ^ accumulator) & (accumulator2 ^ data);

            data = Vector.LoadUnsafe(ref ptr, index + (nuint) Vector<T>.Count);
            accumulator = accumulator2 + data;
            overflowTracking |= (accumulator ^ accumulator2) & (accumulator ^ data);

            data = Vector.LoadUnsafe(ref ptr, index + (nuint) Vector<T>.Count * 2);
            accumulator2 = accumulator + data;
            overflowTracking |= (accumulator2 ^ accumulator) & (accumulator2 ^ data);

            data = Vector.LoadUnsafe(ref ptr, index + (nuint) Vector<T>.Count * 3);
            accumulator = accumulator2 + data;
            overflowTracking |= (accumulator ^ accumulator2) & (accumulator ^ data);

            if ((overflowTracking & overflowTestVector) != Vector<T>.Zero)
            {
                Diagnostics.ThrowHelper.ThrowOverflowException();
            }

            index += (nuint) Vector<T>.Count * 4;
        } while (index < limit);

        // Process remaining vectors, if any, without unrolling
        limit = length - (nuint) Vector<T>.Count;
        if (index < limit)
        {
            Vector<T> overflowTracking = Vector<T>.Zero;

            do
            {
                Vector<T> data = Vector.LoadUnsafe(ref ptr, index);
                Vector<T> accumulator2 = accumulator + data;
                overflowTracking |= (accumulator2 ^ accumulator) & (accumulator2 ^ data);
                accumulator = accumulator2;

                index += (nuint) Vector<T>.Count;
            } while (index < limit);

            if ((overflowTracking & overflowTestVector) != Vector<T>.Zero)
            {
                Diagnostics.ThrowHelper.ThrowOverflowException();
            }
        }

        // Add the elements in the vector horizontally.
        // Vector.Sum doesn't perform overflow checking, instead add elements individually.
        T result = T.Zero;
        for (int i = 0; i < Vector<T>.Count; i++)
        {
            checked
            {
                result += accumulator[i];
            }
        }

        // Add any remaining elements
        while (index < length)
        {
            checked
            {
                result += Unsafe.Add(ref ptr, index);
            }

            index++;
        }

        return result;
    }
}