using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using NullGC.Allocators;

namespace NullGC.Collections;

public struct SlidingWindow<T> : IDisposable where T : unmanaged, INumber<T>
{
    public readonly ref struct CurrentWindow
    {
        public CurrentWindow(T sum, ref readonly ValueFixedSizeDeque<T> window)
        {
            Sum = sum;
            Window = ref window;
        }

        public readonly T Sum;
        public readonly int Count => Window.Count;
        public readonly ref readonly ValueFixedSizeDeque<T> Window;
    }

    private ValueFixedSizeDeque<T> _wnd;
    private T _wndSum;

    public SlidingWindow(int windowSize, int allocatorProviderId = (int) AllocatorTypes.Default)
    {
        Guard.IsGreaterThan(windowSize, 0);
        _wnd = new ValueFixedSizeDeque<T>(windowSize, allocatorProviderId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [UnscopedRef]
    public CurrentWindow Update(T newItem)
    {
        if (_wnd.PushBack(newItem, out var e)) _wndSum -= e;

        _wndSum += newItem;
        return new CurrentWindow(_wndSum, in _wnd);
    }

    public void Dispose() => _wnd.Dispose();
}