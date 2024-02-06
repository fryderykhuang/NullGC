using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using NullGC.Allocators;

namespace NullGC.Collections;

public struct SlidingTimeWindow<T> : IExplicitOwnership<SlidingTimeWindow<T>> where T : unmanaged, INumber<T>
{
    private ValueFixedSizeDeque<Bucket> _wnd;

    private readonly int _resolutionMs;

    public readonly ref struct CurrentWindow
    {
        public readonly T Sum;
        public readonly int Count;
        public readonly ref readonly ValueFixedSizeDeque<Bucket> Buckets;

        public CurrentWindow(ref readonly ValueFixedSizeDeque<Bucket> currentWindow, T sum, int count)
        {
            Buckets = ref currentWindow;
            Sum = sum;
            Count = count;
        }
    }

    public struct Bucket
    {
        public Bucket(T sum, int count)
        {
            _sum = sum;
            _count = count;
        }

        internal Bucket(T sum)
        {
            _sum = sum;
            _count = 1;
        }

        internal T _sum;
        internal int _count;
        public T Sum => _sum;
        public int Count => _count;
    }

    private SlidingTimeWindow(ValueFixedSizeDeque<Bucket> wnd, int resolutionMs, long lastBucket, T wndSum,
        int wndCount, Bucket noDataBucket)
    {
        _wnd = wnd;
        _resolutionMs = resolutionMs;
        _lastBucket = lastBucket;
        _wndSum = wndSum;
        _wndCount = wndCount;
        _noDataBucket = noDataBucket;
    }

    /// <summary>
    /// </summary>
    /// <param name="resolutionMs">differential resolution in milliseconds</param>
    /// <param name="windowSize">
    ///     Count of <paramref name="resolutionMs" />-sized buckets to be used as the size of integral
    ///     window.
    /// </param>
    /// <param name="noDataBucket">The filler bucket for the time duration when no data is present.</param>
    /// <param name="allocatorProviderId"></param>
    public SlidingTimeWindow(int resolutionMs, int windowSize, Bucket noDataBucket,
        int allocatorProviderId = (int) AllocatorTypes.Default)
    {
        _resolutionMs = resolutionMs;
        _wnd = new ValueFixedSizeDeque<Bucket>(windowSize, allocatorProviderId);
        _noDataBucket = noDataBucket;
    }

    private long _lastBucket;
    private T _wndSum;
    private int _wndCount;
    private readonly Bucket _noDataBucket;

    [UnscopedRef]
    public CurrentWindow Update(T newItem)
    {
        var nowBucket = Environment.TickCount64 / _resolutionMs;

        if (_wnd.IsEmpty)
        {
            _wnd.AddBack(new Bucket(newItem));
        }
        else
        {
            var deltaBucket = Math.Min(_wnd.Capacity, nowBucket - _lastBucket);
            if (deltaBucket > 0)
            {
                for (var i = 0; i < deltaBucket - 1; i++)
                    if (_wnd.PushBack(_noDataBucket, out var oldBucket))
                    {
                        _wndSum -= oldBucket._sum;
                        _wndCount -= oldBucket._count;
                    }

                if (_wnd.PushBack(new Bucket(newItem), out var oldBucket2))
                {
                    _wndSum -= oldBucket2._sum;
                    _wndCount -= oldBucket2._count;
                }
            }
            else
            {
                _wnd.TailRef._sum += newItem;
                _wnd.TailRef._count++;
            }
        }

        _wndSum += newItem;
        _wndCount++;
        _lastBucket = nowBucket;

        return new CurrentWindow(ref _wnd, _wndSum, _wndCount);
    }

    public SlidingTimeWindow<T> Borrow()
    {
        return new SlidingTimeWindow<T>(_wnd.Borrow(), _resolutionMs, _lastBucket, _wndSum, _wndCount, _noDataBucket);
    }

    public SlidingTimeWindow<T> Take()
    {
        return new SlidingTimeWindow<T>(_wnd.Take(), _resolutionMs, _lastBucket, _wndSum, _wndCount, _noDataBucket);
    }

    public void Dispose()
    {
        _wnd.Dispose();
    }
}