// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#define TARGET_64BIT // sacrifice performance on 32bit target. (Github Actions does not install x86 dotnet SDK) 
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;
using NullGC.Allocators;
using NullGC.Collections.Extensions;
using NullGC.Diagnostics;
using NullGC.Linq;

namespace NullGC.Collections;

// modified from the code of .NET Core Dictionary<,>.
[DebuggerDisplay("Count = {Count}, IsInitialized = {IsInitialized}")]
public struct ValueDictionary<TKey, TValue> : IDictionary<TKey, TValue>,
    ISingleDisposable<ValueDictionary<TKey, TValue>>,
    ILinqEnumerable<ValueDictionary<TKey, TValue>.Entry, ValueDictionary<TKey, TValue>.Enumerator>,
    ICollection<ValueDictionary<TKey, TValue>.Entry>, IReadOnlyCollection<ValueDictionary<TKey, TValue>.Entry>
    where TKey : unmanaged where TValue : unmanaged
{
    private ValueArray<int> _buckets;
    private ValueArray<Entry> _entries;
#if TARGET_64BIT
    private ulong _fastModMultiplier;
#endif
    private int _count;
    private int _freeList;
    private int _freeCount;
    private readonly IEqualityComparer<TKey>? _comparer;
    private const int FreeListBaseIndex = -3;

    private ValueDictionary(ValueArray<int> buckets, ValueArray<Entry> entries,
#if TARGET_64BIT
        ulong fastModMultiplier,
#endif
        int count,
        int freeList, int freeCount, IEqualityComparer<TKey>? comparer)
    {
        _buckets = buckets;
        _entries = entries;
#if TARGET_64BIT
        _fastModMultiplier = fastModMultiplier;
#endif
        _count = count;
        _freeList = freeList;
        _freeCount = freeCount;
        _comparer = comparer;
    }

    public readonly bool IsInitialized => _buckets.IsInitialized && _entries.IsInitialized;
    public readonly KeyCollection Keys => new(this);
    public readonly ValueCollection Values => new(this);

    readonly ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

    readonly ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;


    public ValueDictionary() : this(0, null, (int) AllocatorTypes.Default)
    {
    }

    public ValueDictionary(AllocatorTypes allocatorProviderId) : this(0, null, (int) allocatorProviderId)
    {
    }

    public ValueDictionary(int capacity, int allocatorProviderId = (int) AllocatorTypes.Default) : this(capacity,
        null, allocatorProviderId)
    {
    }

    public ValueDictionary(int capacity, AllocatorTypes allocatorProviderId) : this(capacity,
        null, (int) allocatorProviderId)
    {
    }

    public ValueDictionary(IEqualityComparer<TKey>? comparer,
        int allocatorProviderId = (int) AllocatorTypes.Default) : this(0, comparer,
        allocatorProviderId)
    {
    }

    public ValueDictionary(int capacity, IEqualityComparer<TKey>? comparer,
        AllocatorTypes allocatorProviderId) : this(capacity, comparer, (int) allocatorProviderId)
    {
    }

    public ValueDictionary(int capacity, IEqualityComparer<TKey>? comparer,
        int allocatorProviderId = (int) AllocatorTypes.Default)
    {
        if (capacity < 0) ThrowHelper.ThrowArgumentOutOfRangeException("capacity");

        _buckets = _buckets.WithAllocationProviderId(allocatorProviderId);

        if (capacity > 0)
        {
            Initialize(capacity);
        }
        else
        {
        }

        if (comparer is not null && // first check for null to avoid forcing default comparer instantiation unnecessarily
            !comparer.Equals(EqualityComparer<TKey>.Default))
            _comparer = comparer;
    }

    public ValueDictionary(IDictionary<TKey, TValue> dictionary) : this(dictionary, null)
    {
    }

    public ValueDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey>? comparer,
        int allocatorProviderId = (int) AllocatorTypes.Default) :
        this(dictionary.Count, comparer, allocatorProviderId)
    {
        Guard.IsNotNull(dictionary, nameof(dictionary));

#pragma warning disable CS8631 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match constraint type.
        AddRange(dictionary);
#pragma warning restore CS8631 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match constraint type.
    }

    public ValueDictionary(ValueDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey>? comparer = null,
        int allocatorProviderId = (int) AllocatorTypes.Default) : this(dictionary.Count, comparer,
        allocatorProviderId)
    {
        AddRange(dictionary);
        _comparer = comparer;
    }

    public ValueDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection,
        IEqualityComparer<TKey>? comparer = null, int allocatorProviderId = (int) AllocatorTypes.Default) :
        this((collection as ICollection<KeyValuePair<TKey, TValue>>)?.Count ?? 0, comparer, allocatorProviderId)
    {
        // ReSharper disable once PossibleMultipleEnumeration
        Guard.IsNotNull(collection, nameof(collection));

        // ReSharper disable once PossibleMultipleEnumeration
        AddRange(collection);
    }

    public void AddRange<TCollection>(TCollection enumerable)
        where TCollection : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        // It is likely that the passed-in enumerable is ValueDictionary<TKey,TValue>. When this is the case,
        // avoid the enumerator allocation and overhead by looping through the entries array directly.
        // We only do this when dictionary is RefDictionary<TKey,TValue> and not a subclass, to maintain
        // back-compat with subclasses that may have overridden the enumerator behavior.
        if (typeof(TCollection) == typeof(ValueDictionary<TKey, TValue>))
        {
            ref var source =
                ref Unsafe.As<TCollection, ValueDictionary<TKey, TValue>>(ref enumerable);

            if (source.Count == 0) return;

            // This is not currently a true .AddRange as it needs to be an initialized dictionary
            // of the correct size, and also an empty dictionary with no current entities (and no argument checks).
            Debug.Assert(_entries.Length >= source.Count);
            Debug.Assert(_count == 0);

            ref var oldEntries = ref source._entries;
            // ReSharper disable once PossibleUnintendedReferenceComparison
            if (source._comparer == _comparer)
            {
                // If comparers are the same, we can copy _entries without rehashing.
                CopyEntries(oldEntries, source._count);
                return;
            }

            // Comparers differ need to rehash all the entries via Add
            var count = source._count;
            for (var i = 0; i < count; i++)
                // Only copy if an entry
                if (oldEntries.GetRefUnchecked(i).Next >= -1)
                    Add(oldEntries.GetRefUnchecked(i).Key, oldEntries.GetRefUnchecked(i).Value);

            return;
        }

        // We similarly special-case KVP<>[] and List<KVP<>>, as they're commonly used to seed dictionaries, and
        // we want to avoid the enumerator costs (e.g. allocation) for them as well. Extract a span if possible.
        ReadOnlySpan<KeyValuePair<TKey, TValue>> span;
        if (enumerable is KeyValuePair<TKey, TValue>[] array)
        {
            span = array;
        }
        else if (enumerable.GetType() == typeof(List<KeyValuePair<TKey, TValue>>))
        {
            span = CollectionsMarshal.AsSpan(Unsafe.As<TCollection, List<KeyValuePair<TKey, TValue>>>(ref enumerable));
        }
        else
        {
            // Fallback path for all other enumerables
            foreach (var pair in enumerable) Add(pair.Key, pair.Value);

            return;
        }

        // We got a span. Add the elements to the dictionary.
        foreach (var pair in span) Add(pair.Key, pair.Value);
    }

    public readonly IEqualityComparer<TKey> Comparer => _comparer ?? EqualityComparer<TKey>.Default;

    bool ICollection<Entry>.Remove(Entry item)
    {
        return CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException<bool>();
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException<bool>();
    }

    public readonly int Count => _count - _freeCount;
    readonly int? IMaybeCountable.MaxCount => Count;
    public readonly bool IsReadOnly => false;

    readonly bool ICollection<Entry>.IsReadOnly => false;

    public TValue this[TKey key]
    {
        readonly get
        {
            ref var value = ref FindValue(key);
            if (!Unsafe.IsNullRef(ref value)) return value;

            ThrowHelper.ThrowKeyNotFoundException(key);
            return Unsafe.NullRef<TValue>();
        }
        set
        {
            // ReSharper disable once RedundantAssignment
            var modified = TryInsert(key, value, InsertionBehavior.OverwriteExisting);
            Debug.Assert(modified);
        }
    }


    public readonly ref TValue GetValueRefOrNullRef(TKey key, out bool exists)
    {
        ref var ret = ref FindValue(key);
        exists = !Unsafe.IsNullRef(ref ret);
        return ref ret;
    }

    public ref TValue GetValueRef(TKey key)
    {
        ref var ret = ref FindValue(key);
        if (Unsafe.IsNullRef(ref ret))
            ThrowHelper.ThrowKeyNotFoundException(key);
        return ref ret;
    }

    public ref TValue GetValueRefOrNullRef(TKey key)
    {
        return ref FindValue(key);
    }

    public void Add(TKey key, TValue value)
    {
        // ReSharper disable once RedundantAssignment
        var modified = TryInsert(key, value, InsertionBehavior.ThrowOnExisting);
        Debug.Assert(modified); // If there was an existing key and the Add failed, an exception will already have been thrown.
    }

    void ICollection<Entry>.Add(Entry item)
    {
        CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
        CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();
    }

    public void Clear()
    {
        if (_count > 0)
        {
            Debug.Assert(_buckets.IsInitialized);
            Debug.Assert(_entries.IsInitialized);

            ValueArray.Clear(_buckets);
            ValueArray.Clear(_entries, 0, _count);
            _count = 0;
            _freeList = -1;
            _freeCount = 0;
        }
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException<bool>();
    }

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        CopyTo(array, arrayIndex);
    }

    bool ICollection<Entry>.Contains(Entry item)
    {
        return CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException<bool>();
    }

    void ICollection<Entry>.CopyTo(Entry[] array, int arrayIndex)
    {
        CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();
    }

    public bool ContainsKey(TKey key)
    {
        return !Unsafe.IsNullRef(ref FindValue(key));
    }

    public bool ContainsValue(TValue value)
    {
        for (var i = 0; i < _count; i++)
            if (_entries.GetRefUnchecked(i).Next >= -1 &&
                EqualityComparer<TValue>.Default.Equals(_entries.GetRefUnchecked(i).Value, value))
                return true;

        return false;
    }

    private void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
    {
        Guard.IsNotNull(array, nameof(array));

        if ((uint) index > (uint) array.Length) ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();

        if (array.Length - index < Count) ThrowHelper.ThrowArgumentException("Arg_ArrayPlusOffTooSmall");

        var count = _count;
        for (var i = 0; i < count; i++)
            if (_entries.GetRefUnchecked(i).Next >= -1)
                array[index++] = new KeyValuePair<TKey, TValue>(_entries.GetRefUnchecked(i).Key,
                    _entries.GetRefUnchecked(i).Value);
    }

    readonly IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
        return CommunityToolkit.Diagnostics.ThrowHelper
            .ThrowNotSupportedException<IEnumerator<KeyValuePair<TKey, TValue>>>();
    }

    public readonly Enumerator GetEnumerator()
    {
        return new Enumerator(this);
    }

    readonly IEnumerator<Entry> IEnumerable<Entry>.GetEnumerator()
    {
        return Count == 0 ? GenericEmptyEnumerator<Entry>.Instance : GetEnumerator();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly ref TValue FindValue(TKey key)
    {
        ref var entry = ref Unsafe.NullRef<Entry>();
        if (_buckets.IsInitialized)
        {
            if (_comparer == null)
            {
                var hashCode = (uint) key.GetHashCode();
                var i = GetBucket(hashCode);
                uint collisionCount = 0;

                i--; // Value in _buckets is 1-based; subtract 1 from i. We do it here so it fuses with the following conditional.
                do
                {
                    // Should be a while loop https://github.com/dotnet/runtime/issues/9422
                    // Test in if to drop range check for following array access
                    if ((uint) i >= (uint) _entries.Length) goto ReturnNotFound;

                    entry = ref _entries.GetRefUnchecked(i);
                    if (entry.HashCode == hashCode && EqualityComparer<TKey>.Default.Equals(entry.Key, key))
                        goto ReturnFound;

                    i = entry.Next;

                    collisionCount++;
                } while (collisionCount <= (uint) _entries.Length);

                // The chain of entries forms a loop; which means a concurrent update has happened.
                // Break out of the loop and throw, rather than looping forever.
                goto ConcurrentOperation;
            }
            else
            {
                Debug.Assert(_comparer is not null);
                var hashCode = (uint) _comparer.GetHashCode(key);
                var i = GetBucket(hashCode);
                uint collisionCount = 0;
                i--; // Value in _buckets is 1-based; subtract 1 from i. We do it here so it fuses with the following conditional.
                do
                {
                    // Should be a while loop https://github.com/dotnet/runtime/issues/9422
                    // Test in if to drop range check for following array access
                    if ((uint) i >= (uint) _entries.Length) goto ReturnNotFound;

                    entry = ref _entries.GetRefUnchecked(i);
                    if (entry.HashCode == hashCode && _comparer.Equals(entry.Key, key)) goto ReturnFound;

                    i = entry.Next;

                    collisionCount++;
                } while (collisionCount <= (uint) _entries.Length);

                // The chain of entries forms a loop; which means a concurrent update has happened.
                // Break out of the loop and throw, rather than looping forever.
                goto ConcurrentOperation;
            }
        }

        goto ReturnNotFound;

        ConcurrentOperation:
        ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
        ReturnFound:
        ref var value = ref entry.Value;
        Return:
        return ref value;
        ReturnNotFound:
        value = ref Unsafe.NullRef<TValue>();
        goto Return;
    }

    private int Initialize(int capacity, bool noDispose = false)
    {
        var size = HashHelpers.GetPrime(capacity);
        // Assign member variables after both arrays allocated to guard against corruption from OOM if second fails
        var buckets = new ValueArray<int>(size, _buckets.AllocatorProviderId);
        var entries = new ValueArray<Entry>(size, _buckets.AllocatorProviderId);
        _freeList = -1;
#if TARGET_64BIT
        _fastModMultiplier = HashHelpers.GetFastModMultiplier((uint) size);
#endif

        if (!noDispose)
        {
            _buckets.Dispose();
            _entries.Dispose();
        }

        _buckets = buckets;
        _entries = entries;
        return size;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryInsert(TKey key, TValue value, InsertionBehavior behavior)
    {
        // NOTE: this method is mirrored in CollectionsMarshal.GetValueRefOrAddDefault below.
        // If you make any changes here, make sure to keep that version in sync as well.

        if (!_buckets.IsInitialized) Initialize(0);

        var comparer = _comparer;
        var hashCode = (uint) (comparer?.GetHashCode(key) ?? key.GetHashCode());

        uint collisionCount = 0;
        ref var bucket = ref GetBucket(hashCode);
        var i = bucket - 1; // Value in _buckets is 1-based

        if (comparer == null)
        {
            while (true)
            {
                // Should be a while loop https://github.com/dotnet/runtime/issues/9422
                // Test uint in if rather than loop condition to drop range check for following array access
                if ((uint) i >= (uint) _entries.Length) break;

                if (_entries.GetRefUnchecked(i).HashCode == hashCode &&
                    EqualityComparer<TKey>.Default.Equals(_entries.GetRefUnchecked(i).Key, key))
                {
                    if (behavior == InsertionBehavior.OverwriteExisting)
                    {
                        _entries.GetRefUnchecked(i).Value = value;
                        return true;
                    }

                    if (behavior == InsertionBehavior.ThrowOnExisting)
                        ThrowHelper.ThrowAddingDuplicateWithKeyArgumentException(key);

                    return false;
                }

                i = _entries.GetRefUnchecked(i).Next;

                collisionCount++;
                if (collisionCount > (uint) _entries.Length)
                    // The chain of entries forms a loop; which means a concurrent update has happened.
                    // Break out of the loop and throw, rather than looping forever.
                    ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
            }
        }
        else
        {
            Debug.Assert(comparer is not null);
            while (true)
            {
                // Should be a while loop https://github.com/dotnet/runtime/issues/9422
                // Test uint in if rather than loop condition to drop range check for following array access
                if ((uint) i >= (uint) _entries.Length) break;

                if (_entries.GetRefUnchecked(i).HashCode == hashCode &&
                    comparer.Equals(_entries.GetRefUnchecked(i).Key, key))
                {
                    if (behavior == InsertionBehavior.OverwriteExisting)
                    {
                        _entries.GetRefUnchecked(i).Value = value;
                        return true;
                    }

                    if (behavior == InsertionBehavior.ThrowOnExisting)
                        ThrowHelper.ThrowAddingDuplicateWithKeyArgumentException(key);

                    return false;
                }

                i = _entries.GetRefUnchecked(i).Next;

                collisionCount++;
                if (collisionCount > (uint) _entries.Length)
                    // The chain of entries forms a loop; which means a concurrent update has happened.
                    // Break out of the loop and throw, rather than looping forever.
                    ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
            }
        }

        int index;
        if (_freeCount > 0)
        {
            index = _freeList;
            Debug.Assert(FreeListBaseIndex - _entries.GetRefUnchecked(_freeList).Next >= -1,
                "shouldn't overflow because `next` cannot underflow");
            _freeList = FreeListBaseIndex - _entries.GetRefUnchecked(_freeList).Next;
            _freeCount--;
        }
        else
        {
            var count = _count;
            if (count == _entries.Length)
            {
                Resize();
                bucket = ref GetBucket(hashCode);
            }

            index = count;
            _count = count + 1;
        }

        ref var entry = ref _entries.GetRefUnchecked(index);
        entry.HashCode = hashCode;
        entry.Next = bucket - 1; // Value in _buckets is 1-based
        entry.Key = key;
        entry.Value = value;
        bucket = index + 1; // Value in _buckets is 1-based

        return true;
    }

    // public ref TValue GetValueRefOrAddDefault(TKey key, out bool exists)
    // {
    //     ref var item = ref GetValueRef(key, out exists);
    //     if (Unsafe.IsNullRef(ref item))
    //     {
    //         TryInsert(key, default, InsertionBehavior.ThrowOnExisting);
    //         return ref GetValueRef(key, out _);
    //     }
    //
    //     return ref item;
    // }

    #region CollectionsMarshalHelper

    public ref TValue GetValueRefOrAddDefault(TKey key, out bool exists)
    {
        // NOTE: this method is mirrored by RefDictionary<TKey, TValue>.TryInsert above.
        // If you make any changes here, make sure to keep that version in sync as well.

        if (!_buckets.IsInitialized) Initialize(0);

        ref var entries = ref _entries;

        var hashCode = (uint) (_comparer?.GetHashCode(key) ?? key.GetHashCode());

        uint collisionCount = 0;
        ref var bucket = ref GetBucket(hashCode);
        var i = bucket - 1; // Value in _buckets is 1-based

        if (_comparer == null)
        {
            while (true)
            {
                // Should be a while loop https://github.com/dotnet/runtime/issues/9422
                // Test uint in if rather than loop condition to drop range check for following array access
                if ((uint) i >= (uint) entries.Length) break;

                if (entries.GetRefUnchecked(i).HashCode == hashCode &&
                    EqualityComparer<TKey>.Default.Equals(entries.GetRefUnchecked(i).Key, key))
                {
                    exists = true;

                    return ref entries.GetRefUnchecked(i).Value;
                }

                i = entries.GetRefUnchecked(i).Next;

                collisionCount++;
                if (collisionCount > (uint) entries.Length)
                    // The chain of entries forms a loop; which means a concurrent update has happened.
                    // Break out of the loop and throw, rather than looping forever.
                    ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
            }
        }
        else
        {
            Debug.Assert(_comparer is not null);
            while (true)
            {
                // Should be a while loop https://github.com/dotnet/runtime/issues/9422
                // Test uint in if rather than loop condition to drop range check for following array access
                if ((uint) i >= (uint) entries.Length) break;

                if (entries.GetRefUnchecked(i).HashCode == hashCode &&
                    _comparer.Equals(entries.GetRefUnchecked(i).Key, key))
                {
                    exists = true;

                    return ref entries.GetRefUnchecked(i).Value;
                }

                i = entries.GetRefUnchecked(i).Next;

                collisionCount++;
                if (collisionCount > (uint) entries.Length)
                    // The chain of entries forms a loop; which means a concurrent update has happened.
                    // Break out of the loop and throw, rather than looping forever.
                    ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
            }
        }

        int index;
        if (_freeCount > 0)
        {
            index = _freeList;
            Debug.Assert(FreeListBaseIndex - entries.GetRefUnchecked(_freeList).Next >= -1,
                "shouldn't overflow because `next` cannot underflow");
            _freeList = FreeListBaseIndex - entries.GetRefUnchecked(_freeList).Next;
            _freeCount--;
        }
        else
        {
            var count = _count;
            if (count == entries.Length)
            {
                Resize();
                bucket = ref GetBucket(hashCode);
            }

            index = count;
            _count = count + 1;
            entries = ref _entries;
        }

        ref var entry = ref entries.GetRefUnchecked(index);
        entry.HashCode = hashCode;
        entry.Next = bucket - 1; // Value in _buckets is 1-based
        entry.Key = key;
        entry.Value = default!;
        bucket = index + 1; // Value in _buckets is 1-based
        exists = false;

        return ref entry.Value;
    }

    #endregion

    private void Resize() => Resize(HashHelpers.ExpandPrime(_count));

    private void Resize(int newSize)
    {
        TraceOn.MemAlloc(nameof(ValueDictionary<TKey, TValue>),
            $"Before resize {nameof(_entries)}={_entries} {nameof(newSize)}={newSize}");
        Debug.Assert(_entries.IsInitialized);
        Debug.Assert(newSize >= _entries.Length);
        var count = _count;
        newSize = _entries.Grow(newSize, newSize, false);
        // Assign member variables after both arrays allocated to guard against corruption from OOM if second fails
        _buckets.Grow(newSize, newSize, true);
        _buckets.Clear();
#if TARGET_64BIT
        _fastModMultiplier = HashHelpers.GetFastModMultiplier((uint) newSize);
#endif
        for (var i = 0; i < count; i++)
            if (_entries.GetRefUnchecked(i).Next >= -1)
            {
                ref var bucket = ref GetBucket(_entries.GetRefUnchecked(i).HashCode);
                _entries.GetRefUnchecked(i).Next = bucket - 1; // Value in _buckets is 1-based
                bucket = i + 1;
            }
    }

    public bool Remove(TKey key)
    {
        // The overload Remove(TKey key, out TValue value) is a copy of this method with one additional
        // statement to copy the value for entry being removed into the output parameter.
        // Code has been intentionally duplicated for performance reasons.

        if (_buckets.IsInitialized)
        {
            Debug.Assert(_entries.IsInitialized);
            uint collisionCount = 0;

            var comparer = _comparer;
            var hashCode = (uint) (comparer?.GetHashCode(key) ?? key.GetHashCode());

            ref var bucket = ref GetBucket(hashCode);
            var last = -1;
            var i = bucket - 1; // Value in buckets is 1-based
            while (i >= 0)
            {
                ref var entry = ref _entries.GetRefUnchecked(i);

                if (entry.HashCode == hashCode &&
                    (comparer?.Equals(entry.Key, key) ?? EqualityComparer<TKey>.Default.Equals(entry.Key, key)))
                {
                    if (last < 0)
                        bucket = entry.Next + 1; // Value in buckets is 1-based
                    else
                        _entries.GetRefUnchecked(last).Next = entry.Next;

                    Debug.Assert(FreeListBaseIndex - _freeList < 0,
                        "shouldn't underflow because max hashtable length is MaxPrimeArrayLength = 0x7FEFFFFD(2146435069) _freelist underflow threshold 2147483646");
                    entry.Next = FreeListBaseIndex - _freeList;

                    _freeList = i;
                    _freeCount++;
                    return true;
                }

                last = i;
                i = entry.Next;

                collisionCount++;
                if (collisionCount > (uint) _entries.Length)
                    // The chain of entries forms a loop; which means a concurrent update has happened.
                    // Break out of the loop and throw, rather than looping forever.
                    ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
            }
        }

        return false;
    }

    public bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        // This overload is a copy of the overload Remove(TKey key) with one additional
        // statement to copy the value for entry being removed into the output parameter.
        // Code has been intentionally duplicated for performance reasons.

        if (_buckets.IsInitialized)
        {
            Debug.Assert(_entries.IsInitialized);
            uint collisionCount = 0;

            var comparer = _comparer;
            var hashCode = (uint) (comparer?.GetHashCode(key) ?? key.GetHashCode());

            ref var bucket = ref GetBucket(hashCode);
            var last = -1;
            var i = bucket - 1; // Value in buckets is 1-based
            while (i >= 0)
            {
                ref var entry = ref _entries.GetRefUnchecked(i);

                if (entry.HashCode == hashCode && (comparer == null
                        ? EqualityComparer<TKey>.Default.Equals(entry.Key, key)
                        : comparer.Equals(entry.Key, key)))
                {
                    if (last < 0)
                        bucket = entry.Next + 1; // Value in buckets is 1-based
                    else
                        _entries.GetRefUnchecked(last).Next = entry.Next;

                    value = entry.Value;

                    Debug.Assert(FreeListBaseIndex - _freeList < 0,
                        "shouldn't underflow because max hashtable length is MaxPrimeArrayLength = 0x7FEFFFFD(2146435069) _freelist underflow threshold 2147483646");
                    entry.Next = FreeListBaseIndex - _freeList;

                    _freeList = i;
                    _freeCount++;
                    return true;
                }

                last = i;
                i = entry.Next;

                collisionCount++;
                if (collisionCount > (uint) _entries.Length)
                    // The chain of entries forms a loop; which means a concurrent update has happened.
                    // Break out of the loop and throw, rather than looping forever.
                    ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
            }
        }

        value = default;
        return false;
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        ref var valRef = ref FindValue(key);
        if (!Unsafe.IsNullRef(ref valRef))
        {
            value = valRef;
            return true;
        }

        value = default;
        return false;
    }

    public bool TryAdd(TKey key, TValue value)
    {
        return TryInsert(key, value, InsertionBehavior.None);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<Entry>) this).GetEnumerator();
    }

    public int EnsureCapacity(int capacity)
    {
        if (capacity < 0) ThrowHelper.ThrowArgumentOutOfRangeException("capacity");

        var currentCapacity = _entries.Length;
        if (currentCapacity >= capacity) return currentCapacity;

        if (!_buckets.IsInitialized) return Initialize(capacity);

        var newSize = HashHelpers.GetPrime(capacity);
        Resize(newSize);
        return newSize;
    }

    public void TrimExcess()
    {
        TrimExcess(Count);
    }

    public void TrimExcess(int capacity)
    {
        Guard.IsGreaterThanOrEqualTo(capacity, Count);

        var newSize = HashHelpers.GetPrime(capacity);
        if (newSize >= _entries.Length) return;

        var oldEntries = _entries;
        var oldBuckets = _buckets;
        var oldCount = _count;
        Initialize(newSize, true);

        Debug.Assert(oldEntries.IsInitialized);

        CopyEntries(oldEntries, oldCount);
        oldEntries.Dispose();
        oldBuckets.Dispose();
    }

    private void CopyEntries(ValueArray<Entry> entries, int count)
    {
        Debug.Assert(_entries.IsInitialized);

        var newCount = 0;
        for (var i = 0; i < count; i++)
        {
            var hashCode = entries.GetRefUnchecked(i).HashCode;
            if (entries.GetRefUnchecked(i).Next >= -1)
            {
                ref var entry = ref _entries.GetRefUnchecked(newCount);
                entry = entries.GetRefUnchecked(i);
                ref var bucket = ref GetBucket(hashCode);
                entry.Next = bucket - 1; // Value in _buckets is 1-based
                bucket = newCount + 1;
                newCount++;
            }
        }

        _count = newCount;
        _freeCount = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly ref int GetBucket(uint hashCode)
    {
#if TARGET_64BIT
        return ref _buckets.GetRefUnchecked((int) HashHelpers.FastMod(hashCode, (uint) _buckets.Length,
            _fastModMultiplier));
#else
        return ref _buckets.GetRefUnchecked((int) (hashCode % _buckets.Length));
#endif
    }

    public struct Entry
    {
        internal uint HashCode;

        /// <summary>
        ///     0-based index of next entry in chain: -1 means end of chain
        ///     also encodes whether this entry _itself_ is part of the free list by changing sign and subtracting 3,
        ///     so -2 means end of free list, -3 means index 0 but on free list, -4 means index 1 but on free list, etc.
        /// </summary>
        internal int Next;

        public TKey Key;
        public TValue Value;
    }

    public readonly struct KeyCollection : ICollection<TKey>, IReadOnlyCollection<TKey>
    {
        private readonly ValueDictionary<TKey, TValue> _dictionary;

        public KeyCollection(ValueDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        // ReSharper disable once MemberHidesStaticFromOuterClass
        public struct Enumerator : ILinqRefEnumerator<TKey>, ILinqValueEnumerator<TKey>, IAddressFixed
        {
            private readonly ValueDictionary<TKey, TValue> _dictionary;
            private int _index;

            internal Enumerator(ValueDictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
                _index = 0;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                while ((uint) _index < (uint) _dictionary._count)
                {
                    ref var entry = ref _dictionary._entries.GetRefUnchecked(_index++);

                    if (entry.Next >= -1) return true;
                }

                _index = _dictionary._count + 1;
                return false;
            }

            public ref TKey Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref _dictionary._entries.GetRefUnchecked(_index - 1).Key;
            }

            public unsafe TKey* CurrentPtr => &_dictionary._entries.Items[_index - 1].Key;

            // ref TKey ILinqRefEnumerator<TKey>.Current => ref _dictionary._entries[_index - 1].Key;
            public int? Count => _dictionary.Count;

            TKey IEnumerator<TKey>.Current => _dictionary._entries.GetRefUnchecked(_index - 1).Key;

            object IEnumerator.Current => _dictionary._entries.GetRefUnchecked(_index - 1).Key;

            public int? MaxCount => _dictionary.Count;

            void IEnumerator.Reset()
            {
                _index = 0;
            }

            public bool SetSkipCount(int count)
            {
                _index = count;
                return true;
            }

            public bool SetTakeCount(int count) => false;
        }

        public Enumerator GetEnumerator() => new(_dictionary);

        IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(TKey item)
        {
            CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();
        }

        public void Clear()
        {
            CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();
        }

        public bool Contains(TKey item)
        {
            return CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException<bool>();
        }

        public void CopyTo(TKey[] array, int arrayIndex)
        {
            CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();
        }

        public bool Remove(TKey item)
        {
            return CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException<bool>();
        }

        public int Count => _dictionary.Count;
        public bool IsReadOnly => true;
    }

    public readonly struct ValueCollection : ICollection<TValue>, IReadOnlyCollection<TValue>
    {
        private readonly ValueDictionary<TKey, TValue> _dictionary;

        public ValueCollection(ValueDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        // ReSharper disable once MemberHidesStaticFromOuterClass
        public struct Enumerator : ILinqRefEnumerator<TValue>, ILinqValueEnumerator<TValue>, IAddressFixed
        {
            private readonly ValueDictionary<TKey, TValue> _dictionary;
            private int _index;

            internal Enumerator(ValueDictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
                _index = 0;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                while ((uint) _index < (uint) _dictionary._count)
                {
                    ref var entry = ref _dictionary._entries.GetRefUnchecked(_index++);

                    if (entry.Next >= -1) return true;
                }

                _index = _dictionary._count + 1;
                return false;
            }

            public ref TValue Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref _dictionary._entries.GetRefUnchecked(_index - 1).Value;
            }

            public unsafe TValue* CurrentPtr => &_dictionary._entries.Items[_index - 1].Value;

            // ref TValue ILinqRefEnumerator<TValue>.Current => ref _dictionary._entries[_index - 1].Value;
            public int? Count => _dictionary.Count;

            TValue IEnumerator<TValue>.Current => _dictionary._entries.GetRefUnchecked(_index - 1).Value;

            object IEnumerator.Current => _dictionary._entries.GetRefUnchecked(_index - 1).Value;

            public int? MaxCount => _dictionary.Count;

            void IEnumerator.Reset() => _index = 0;

            public bool SetSkipCount(int count)
            {
                _index = count;
                return true;
            }

            public bool SetTakeCount(int count) => false;
        }

        public Enumerator GetEnumerator() => new(_dictionary);

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(TValue item)
        {
            CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();
        }

        public void Clear()
        {
            CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();
        }

        public bool Contains(TValue item)
        {
            return CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException<bool>();
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException();
        }

        public bool Remove(TValue item)
        {
            return CommunityToolkit.Diagnostics.ThrowHelper.ThrowNotSupportedException<bool>();
        }

        public int Count => _dictionary.Count;
        public bool IsReadOnly => true;

        public void Dispose()
        {
        }
    }


    public struct Enumerator : ILinqRefEnumerator<Entry>, ILinqValueEnumerator<Entry>, IAddressFixed
    {
        private readonly ValueDictionary<TKey, TValue> _dictionary;
        private int _index;
        private int _current;

        internal Enumerator(ValueDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
            _index = 0;
            _current = default;
        }

        public bool MoveNext()
        {
            // Use unsigned comparison since we set index to dictionary.count+1 when the enumeration ends.
            // dictionary.count+1 could be negative if dictionary.count is int.MaxValue
            while ((uint) _index < (uint) _dictionary._count)
            {
                ref var entry = ref _dictionary._entries.GetRefUnchecked(_index++);

                if (entry.Next >= -1)
                {
                    _current = _index - 1;
                    return true;
                }
            }

            _index = _dictionary._count + 1;
            _current = -1;
            return false;
        }

        public ref Entry Current =>
            ref _current < 0 ? ref Unsafe.NullRef<Entry>() : ref _dictionary._entries.GetRefUnchecked(_current);

        public unsafe Entry* CurrentPtr => _current < 0 ? (Entry*) 0 : &_dictionary._entries.Items[_current];

        Entry IEnumerator<Entry>.Current => _current < 0 ? default : _dictionary._entries.GetUnchecked(_current);

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }

        public void Reset()
        {
            _index = 0;
            _current = -1;
        }

        public int? Count => _dictionary.Count;
        public int? MaxCount => _dictionary.Count;

        public bool SetSkipCount(int count)
        {
            _index = count;
            return true;
        }

        public bool SetTakeCount(int count) => false;
    }

    public ValueDictionary<TKey, TValue> Borrow()
    {
        return new ValueDictionary<TKey, TValue>(_buckets.Borrow(), _entries.Borrow(),
#if TARGET_64BIT
            _fastModMultiplier,
#endif
            _count, _freeList, _freeCount, _comparer);
    }

    public void Dispose()
    {
        _buckets.Dispose();
        _entries.Dispose();
    }

    int? IMaybeCountable.Count => Count;

    internal enum InsertionBehavior : byte
    {
        /// <summary>
        ///     The default insertion behavior.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Specifies that an existing entry with the same key should be overwritten if encountered.
        /// </summary>
        OverwriteExisting = 1,

        /// <summary>
        ///     Specifies that if an existing entry with the same key is encountered, an exception should be thrown.
        /// </summary>
        ThrowOnExisting = 2
    }
}