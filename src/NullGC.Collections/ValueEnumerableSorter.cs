// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;
using NullGC.Allocators;
using NullGC.Collections.Extensions;

namespace NullGC.Collections;

// modified from the .NET Core EnumerableSorter<>
public struct ValueEnumerableSorter<TElement, TKey, TKeySel, TComparer, TNext> : IComparer<int>,
    IValueEnumerableSorter<TElement>,
    IExplicitOwnership<ValueEnumerableSorter<TElement, TKey, TKeySel, TComparer, TNext>>
    where TKeySel : struct
    where TElement : unmanaged
    where TComparer : IComparer<TKey>
    where TKey : unmanaged
    where TNext : struct, IValueEnumerableSorter<TElement>
{
    [Flags]
    private enum Flags
    {
        Descending = 1,
        HasKeySelector = 1 << 1,
        HasNext = 1 << 2
    }

    private readonly TKeySel _keySelector;
    private readonly TComparer _comparer;
    private readonly Flags _flags;
    private int _allocatorProviderId;
    private ValueArray<TKey> _keys;
    private TNext _next;

    public ValueEnumerableSorter(TKeySel? keySelector, TComparer comparer, bool descending, TNext next)
        : this(keySelector, comparer, descending, (int) AllocatorTypes.Default, next)
    {
    }

    public ValueEnumerableSorter(TKeySel? keySelector, TComparer comparer, bool descending, int allocatorProviderId,
        TNext next)
    {
        _flags |= (descending ? Flags.Descending : 0) | Flags.HasNext;
        if (keySelector.HasValue)
        {
            _flags |= Flags.HasKeySelector;
            _keySelector = keySelector.Value;
        }

        _comparer = comparer;
        _allocatorProviderId = allocatorProviderId;
        _next = next;
    }

    public ValueEnumerableSorter(TKeySel? keySelector, TComparer comparer, bool descending,
        int allocatorProviderId = (int) AllocatorTypes.Default)
    {
        _flags |= descending ? Flags.Descending : 0;
        if (keySelector.HasValue)
        {
            _flags |= Flags.HasKeySelector;
            _keySelector = keySelector.Value;
        }

        _comparer = comparer;
        _allocatorProviderId = allocatorProviderId;
    }

    private ValueEnumerableSorter(TKeySel keySelector, TComparer comparer, Flags flags, int allocatorProviderId,
        ValueArray<TKey> keys, TNext next)
    {
        _keySelector = keySelector;
        _comparer = comparer;
        _flags = flags;
        _allocatorProviderId = allocatorProviderId;
        _keys = keys;
        _next = next;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe ValueArray<int> ComputeMap(TElement* elements, int count)
    {
        ComputeKeys(elements, count);
        var map = new ValueArray<int>(count, _allocatorProviderId, true);
        for (var i = 0; i < map.Length; i++) map.Items[i] = i;

        return map;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe ValueArray<int> ComputeMap(Ptr<TElement>* elements, int count)
    {
        ComputeKeys(elements, count);
        var map = new ValueArray<int>(count, _allocatorProviderId, true);
        for (var i = 0; i < map.Length; i++) map.Items[i] = i;

        return map;
    }

    public ValueArray<int> Sort(ValueArray<TElement> elements, int count)
    {
        unsafe
        {
            Guard.IsLessThanOrEqualTo(count, elements.Length);
            var map = ComputeMap(elements.Items, count);
            QuickSort(map.Items, 0, count - 1);
            return map;
        }
    }

    public ValueArray<int> Sort(ValueArray<Ptr<TElement>> elements, int count)
    {
        unsafe
        {
            Guard.IsLessThanOrEqualTo(count, elements.Length);
            var map = ComputeMap(elements.Items, count);
            QuickSort(map.Items, 0, count - 1);
            return map;
        }
    }

    public ValueArray<int> Sort(ValueArray<TElement> elements, int count, int minIdx, int maxIdx)
    {
        Guard.IsLessThanOrEqualTo(count, elements.Length);
        Guard.IsInRange(minIdx, 0, count);
        Guard.IsInRange(maxIdx, 0, count);
        unsafe
        {
            var map = ComputeMap(elements.Items, count);
            PartialQuickSort(map.Items, map.Length, 0, count - 1, minIdx, maxIdx);
            return map;
        }
    }

    public ValueArray<int> Sort(ValueArray<Ptr<TElement>> elements, int count, int minIdx, int maxIdx)
    {
        Guard.IsLessThanOrEqualTo(count, elements.Length);
        Guard.IsInRange(minIdx, 0, count);
        Guard.IsInRange(maxIdx, 0, count);
        unsafe
        {
            var map = ComputeMap(elements.Items, count);
            PartialQuickSort(map.Items, map.Length, 0, count - 1, minIdx, maxIdx);
            return map;
        }
    }

    internal TElement ElementAt(ValueArray<TElement> elements, int count, int idx)
    {
        unsafe
        {
            var map = ComputeMap(elements.Items, count);
            return idx == 0 ? elements.GetUnchecked(Min(map, count)) : elements.GetUnchecked(QuickSelect(map, count - 1, idx));
        }
    }

    // public ValueEnumerableSorter(TKeySel? keySelector, TComparer comparer,
    //     bool descending, TNext? next = null, int allocatorProviderId = (int) AllocatorTypes.Default)
    // {
    //     _keySelector = keySelector;
    //     _comparer = comparer;
    //     _descending = descending;
    //     _allocatorProviderId = allocatorProviderId;
    //     _next = next;
    // }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void ComputeKeys(TElement* items, int count)
    {
        if ((_flags & Flags.HasKeySelector) != 0) //!EqualityComparer<TKeySel>.Default.Equals(_keySelector, default))
        {
            _keys.Dispose();
            _keys = new ValueArray<TKey>(count, _allocatorProviderId, true);
            var keys = _keys.Items;
            if (typeof(TKeySel).IsAssignableTo(typeof(IFuncInvoker<TElement, TKey>)))
                for (var i = 0; i < _keys.Length; i++)
                    keys[i] = ((IFuncInvoker<TElement, TKey>) _keySelector).Invoke(items[i]);
            else if (typeof(TKeySel).IsAssignableTo(typeof(IFuncT1InInvoker<TElement, TKey>)))
                for (var i = 0; i < _keys.Length; i++)
                    keys[i] = ((IFuncT1InInvoker<TElement, TKey>) _keySelector).Invoke(in items[i]);
            else if (typeof(TKeySel).IsAssignableTo(typeof(IFuncT1PtrInvoker<TElement, TKey>)))
                for (var i = 0; i < _keys.Length; i++)
                    keys[i] = ((IFuncT1PtrInvoker<TElement, TKey>) _keySelector).Invoke(&items[i]);
            else
                CommunityToolkit.Diagnostics.ThrowHelper.ThrowInvalidOperationException("Invalid key selector type.");
        }
        else
        {
            // The key selector is our known identity function, which means we don't
            // need to invoke the key selector for every element.  Further, we can just
            // use the original array as the keys (even if count is smaller, as the additional
            // values will just be ignored).
            if (typeof(TKey) != typeof(TElement))
                CommunityToolkit.Diagnostics.ThrowHelper.ThrowInvalidOperationException(
                    "Key type is different from element type.");

            _keys.Dispose();
            _keys = new ValueArray<TKey>((TKey*) items, count);// Unsafe.As<ValueArray<TElement>, ValueArray<TKey>>(ref elements);
        }

        if ((_flags & Flags.HasNext) != 0)
            _next.ComputeKeys(items, count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void ComputeKeys(Ptr<TElement>* items, int count)
    {
        if ((_flags & Flags.HasKeySelector) != 0) //!EqualityComparer<TKeySel>.Default.Equals(_keySelector, default))
        {
            _keys.Dispose();
            _keys = new ValueArray<TKey>(count, _allocatorProviderId, true);
            var keys = _keys.Items;
            if (typeof(TKeySel).IsAssignableTo(typeof(IFuncInvoker<TElement, TKey>)))
                for (var i = 0; i < _keys.Length; i++)
                {
                    var val = items[i].Ref;
                    keys[i] = ((IFuncInvoker<TElement, TKey>) _keySelector).Invoke(val);
                }
            else if (typeof(TKeySel).IsAssignableTo(typeof(IFuncT1InInvoker<TElement, TKey>)))
                for (var i = 0; i < _keys.Length; i++)
                    keys[i] = ((IFuncT1InInvoker<TElement, TKey>) _keySelector).Invoke(in items[i].Ref);
            else if (typeof(TKeySel).IsAssignableTo(typeof(IFuncT1PtrInvoker<TElement, TKey>)))
                for (var i = 0; i < _keys.Length; i++)
                    keys[i] = ((IFuncT1PtrInvoker<TElement, TKey>) _keySelector).Invoke(items[i].Value);
            else
                CommunityToolkit.Diagnostics.ThrowHelper.ThrowInvalidOperationException("Invalid key selector type.");
        }
        else
        {
            // The key selector is our known identity function, which means we don't
            // need to invoke the key selector for every element.  Further, we can just
            // use the original array as the keys (even if count is smaller, as the additional
            // values will just be ignored).
            if (typeof(TKey) != typeof(TElement))
                CommunityToolkit.Diagnostics.ThrowHelper.ThrowInvalidOperationException(
                    "Key type is different from element type.");

            _keys.Dispose();
            _keys = new ValueArray<TKey>(count, _allocatorProviderId, true);
            var keys = _keys.Items;
            for (var i = 0; i < _keys.Length; i++) keys[i] = *(TKey*) items[i].Value;
        }


        if ((_flags & Flags.HasNext) != 0)
            _next.ComputeKeys(items, count);
    }

    public int CompareAnyKeys(int index1, int index2)
    {
        Debug.Assert(_keys.IsAllocated);

        var c = _comparer.Compare(_keys.GetUnchecked(index1), _keys.GetUnchecked(index2));
        if (c == 0)
        {
            if ((_flags & Flags.HasNext) == 0) return index1 - index2; // ensure stability of sort
            return _next.CompareAnyKeys(index1, index2);
        }

        // -c will result in a negative value for int.MinValue (-int.MinValue == int.MinValue).
        // Flipping keys earlier is more likely to trigger something strange in a comparer,
        // particularly as it comes to the sort being stable.
        return (_flags & Flags.Descending) != 0 != c > 0 ? 1 : -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CompareKeys(int index1, int index2)
    {
        return index1 == index2 ? 0 : CompareAnyKeys(index1, index2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe void QuickSort(int* keys, int lo, int hi)
    {
        var span = MemoryMarshal.CreateSpan(ref Unsafe.AsRef<int>(keys + lo), hi - lo + 1);// keys.AsSpan(lo, hi - lo + 1);
        if (/*typeof(TKey).IsValueType &&*/ (_flags & Flags.HasNext) == 0 && typeof(TComparer) == typeof(Comparer<TKey>) &&
            ReferenceEquals(_comparer, Comparer<TKey>.Default))
        {
            // We can use Comparer<TKey>.Default.Compare and benefit from devirtualization and inlining.
            // We can also avoid extra steps to check whether we need to deal with a subsequent tie breaker (_next).
            if ((_flags & Flags.Descending) == 0)
                span.Sort(new DefaultComparerAscending(_keys.Items));
            else
                span.Sort(new DefaultComparerDescending(_keys.Items));
        }
        else
        {
            span.Sort(this);
        }
    }

    // Sorts the k elements between minIdx and maxIdx without sorting all elements
    // Time complexity: O(n + k log k) best and average case. O(n^2) worse case.
    private unsafe void PartialQuickSort(int* map, int length, int left, int right, int minIdx, int maxIdx)
    {
        do
        {
            var i = left;
            var j = right;
            var x = map[i + ((j - i) >> 1)];
            do
            {
                while (i < length && CompareKeys(x, map[i]) > 0) i++;

                while (j >= 0 && CompareKeys(x, map[j]) < 0) j--;

                if (i > j) break;

                if (i < j)
                {
                    var temp = map[i];
                    map[i] = map[j];
                    map[j] = temp;
                }

                i++;
                j--;
            } while (i <= j);

            if (minIdx >= i)
                left = i + 1;
            else if (maxIdx <= j) right = j - 1;

            if (j - left <= right - i)
            {
                if (left < j) PartialQuickSort(map, length, left, j, minIdx, maxIdx);

                left = i;
            }
            else
            {
                if (i < right) PartialQuickSort(map, length, i, right, minIdx, maxIdx);

                right = j;
            }
        } while (left < right);
    }

    // Finds the element that would be at idx if the collection was sorted.
    // Time complexity: O(n) best and average case. O(n^2) worse case.
    public int QuickSelect(ValueArray<int> map, int right, int idx)
    {
        var left = 0;
        do
        {
            var i = left;
            var j = right;
            var x = map.GetUnchecked(i + ((j - i) >> 1));
            do
            {
                while (i < map.Length && CompareKeys(x, map.GetUnchecked(i)) > 0) i++;

                while (j >= 0 && CompareKeys(x, map.GetUnchecked(j)) < 0) j--;

                if (i > j) break;

                if (i < j)
                {
                    
                    var temp = map.GetUnchecked(i);
                    map.GetRefUnchecked(i) = map.GetUnchecked(j);
                    map.GetRefUnchecked(j) = temp;
                }

                i++;
                j--;
            } while (i <= j);

            if (i <= idx)
                left = i + 1;
            else
                right = j - 1;

            if (j - left <= right - i)
            {
                if (left < j) right = j;

                left = i;
            }
            else
            {
                if (i < right) left = i;

                right = j;
            }
        } while (left < right);

        return map.GetUnchecked(idx);
    }

    private int Min(ValueArray<int> map, int count)
    {
        var index = 0;
        for (var i = 1; i < count; i++)
            if (CompareKeys(map.GetUnchecked(i), map.GetUnchecked(index)) < 0)
                index = i;

        return map.GetUnchecked(index);
    }

    public ValueEnumerableSorter<TElement, TKey, TKeySel, TComparer, TNext> Borrow()
    {
        return new ValueEnumerableSorter<TElement, TKey, TKeySel, TComparer, TNext>(_keySelector, _comparer, _flags,
            _allocatorProviderId, _keys.Borrow(), _next);
    }

    public ValueEnumerableSorter<TElement, TKey, TKeySel, TComparer, TNext> Take()
    {
        return new ValueEnumerableSorter<TElement, TKey, TKeySel, TComparer, TNext>(_keySelector, _comparer, _flags,
            _allocatorProviderId, _keys.Take(), _next);
    }

    public void Dispose()
    {
        _keys.Dispose();
        if ((_flags & Flags.HasNext) != 0)
            _next.Dispose();
    }

    private readonly struct DefaultComparerAscending : IComparer<int>
    {
        private readonly unsafe TKey* _keys;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe DefaultComparerAscending(TKey* keys)
        {
            _keys = keys;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Compare(int index1, int index2)
        {
            unsafe
            {
                var c = Comparer<TKey>.Default.Compare(_keys[index1], _keys[index2]);
                return c == 0
                    ? index1 - index2
                    : // ensure stability of sort
                    c;
            }
        }
    }

    private readonly struct DefaultComparerDescending : IComparer<int>
    {
        private readonly unsafe TKey* _keys;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe DefaultComparerDescending(TKey* keys)
        {
            _keys = keys;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Compare(int index1, int index2)
        {
            unsafe
            {
                var c = Comparer<TKey>.Default.Compare(_keys[index2], _keys[index1]);
                return c == 0
                    ? index1 - index2
                    : // ensure stability of sort
                    c;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Compare(int x, int y)
    {
        return CompareAnyKeys(x, y);
    }

    public void SetAllocatorProviderId(int allocatorProviderId)
    {
        _allocatorProviderId = allocatorProviderId;
        if ((_flags & Flags.HasNext) != 0)
            _next.SetAllocatorProviderId(allocatorProviderId);
    }
}