using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;

namespace NullGC;

public struct Option<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option(T value)
    {
        HasValue = true;
        Value = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option()
    {
        Unsafe.SkipInit(out Value);
    }

    public bool HasValue;
    public T Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Option<T>(T value)
    {
        return new Option<T>(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator T(Option<T> option)
    {
        if (!option.HasValue)
            ThrowHelper.ThrowInvalidOperationException("No value available.");
        return option.Value;
    }
}