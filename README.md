[![NullGC](https://github.com/fryderykhuang/NullGC/actions/workflows/main.yml/badge.svg)](https://github.com/fryderykhuang/NullGC/actions/workflows/main.yml)
![NuGet Version](https://img.shields.io/nuget/vpre/NullGC.Abstractions?label=NullGC.Abstractions)
![NuGet Version](https://img.shields.io/nuget/vpre/NullGC.Allocators?label=NullGC.Allocators)
![NuGet Version](https://img.shields.io/nuget/vpre/NullGC.Collections?label=NullGC.Collections)
![NuGet Version](https://img.shields.io/nuget/vpre/NullGC.Linq?label=NullGC.Linq)

# NullGC

High performance unmanaged memory allocator / collection types / LINQ provider for .NET Core. ([Benchmark Results](https://fryderykhuang.github.io/NullGC/))
Most suitable for game development since there will be no latency jitter caused by .NET garbage collection activities.

## Motivation

This project was born mostly out of my curiosity on how far can it go to entirely eliminate garbage collection, also as a side project emerged from an ongoing game engine development. Although .NET background GC is already good at hiding GC stops, still there are some. Also for throughput focused scenarios, there may be huge performance potential when GC is completely out of the equation according to my previous experience on realtime data processing.

## Usage

Currently this project contains 3 components:

1. Unmanaged memory allocator
2. Value type (struct) only collections
3. Linq operators

### Setup

1. Install NuGet package `NullGC.Allocators` and `NullGC.Linq`

2. Setup AllocatorContext:

```csharp
AllocatorContext.SetImplementation(new DefaultAllocatorContextImpl().ConfigureDefault());
```
Allocator context is used internally in `ValueArray<T>` and any code that needs to allocate unmanaged memory.

### Custom collection types

The following types all use unmanaged memory as their internal state store.

* ValueArray&lt;T&gt;
* ValueList&lt;T&gt;
* ValueStack&lt;T&gt;
* ValueQueue&lt;T&gt;
* ValueDictionary&lt;TKey, TValue&gt;
* ValueLinkedList&lt;T&gt;
* ValueFixedSizeDeque&lt;T&gt; (Circular buffer)
* SlidingWindow&lt;T&gt;
* SlidingTimeWindow&lt;T&gt;

*All collection types can be enumerated by ref (`foreach(ref var item in collection)`)

### Memory allocation strategies

Two types of memory allocation strategy are supported:

#### 1. Arena

```csharp
// all 'T' is struct if not specifically mentioned.
using (AllocatorContext.BeginAllocationScope())
{
    // plain old 'new'
    var list = new ValueList<T>();
    var dict = new ValueDictionary<TKey, TValue>();
    
    // let struct T work like a class (T is allocated on the unmanaged heap.)
    var obj = new Allocated<T>();
    ...
} // all value objects are automatically disposed as they go out of scope,
  // no need to explicitly call Dispose().
```

#### 2. Explicit lifetime

You can utilize the unmanaged allocator anywhere like this (including inside of arena scope):

```csharp
// use the overload with parameter 'AllocatorTypes', specify the unscoped, globally available allocator type.
var list = new ValueList<T>(AllocatorTypes.DefaultUnscoped); 
var obj = new Allocated<T>(AllocatorTypes.DefaultUnscoped);
...
// Anywhere after usage:
list.Dispose();
obj.Dispose();
// As long as 'list' is the original one not a copy, it can be disposed saftely multiple times.
list.Dispose(); // ok.
```

### Double-free problem when using explicit lifetime

First of all, collections with unscoped allocator type needs to call `Dispose()` to free the unmanaged memory allocated inside. Typical `Dispose()` implementation will be: If the unmanaged pointer is NULL, ignore; If not NULL, pass the pointer to the native free() function and reset the pointer to NULL. This seems to prevent the double-free, but is it? 

```csharp
struct BadGuy : IDisposable {
    private ValueList<T> _lst;
    public BadGuy(ValueList<T> lst){
        _lst = lst; // '_lst' is a copy of 'list' below.
    }
    
    public void Dispose() {
        _lst.Dispose();
        // '_lst' is now in disposed state, but not the 'list' below,
    }
}

var list = new ValueList<T>(AllocatorTypes.DefaultUnscoped);
using (var badGuy = new BadGuy(list)) {
...   
} // 'badGuy' is dead, however..
... 
list.Dispose(); // Since 'list' is NOT in the disposed state, this will cause the double-free.
```

So to prevent double-free, when these value collections are passed by value, `Borrow()` (from interface `ISingleDisposable<TSelf>`) should be used.

```csharp
void SomeMethod(ValueList<T> lst){ // 'lst' is a copy of 'list' below.
    lst.Dispose(); // Does nothing.
}

var list = new ValueList<T>(AllocatorTypes.DefaultUnscoped);
SomeMethod(list.Borrow()); // The borrowed one's Dispose() is a no-op.
list.Dispose(); // Ok.
```

### Interop with managed object

If you have to use managed object (i.e. class) inside a struct, you can use
`Pinned<T>` to pin the object down so that its address is fixed and can be stored on a non-GC rooted place.

*Since .NET 5 there's a specific heap type for pinned object called [POH](https://devblogs.microsoft.com/dotnet/internals-of-the-poh/), the performance impact will be quite low. 

### Linq

**The fastest LINQ provider as of today** (2024.1).

[Benchmark Results](https://fryderykhuang.github.io/NullGC/) (Auto updated by GitHub Actions, compared with LinqGen/RefLinq/HyperLinq)

Proper usage is with the built-in value typed collections, but good old IEnumerable&lt;T&gt; is also supported. You can still get some benefit on LINQ operators that need to buffer data such as OrderBy.
The LINQ interface has 3 variations:

```csharp
SomeCollection.LinqValue()... // Enumerate by value. All types implement IEnumerable<T> are supported
SomeCollection.LinqRef()...   // Enumerate by ref. Besides value collections, only Enumerators that exposes 'ref T Current' are supported (e.g. normal array types)
SomeCollection.LinqPtr()...   // Enumerate by pointer. Only built-in value typed collections are supported. (Because object address must be fixed to be able to use unmanaged pointer)
```

Most extension methods that needs a delegate type parameter has an overloads with `in` or `ref` modifier to avoid copying too much data if the Linqed type is a big struct.

```csharp
// T is something BIG.
...Where((in T x) => ...).Select((ref T x) => ref x.SomeField)... // all reference, no copy of T
```

Most extension methods has overloads with a `TArg arg` parameter to avoid unnecessary variable capture, thus avoids the allocation of a capture object.

```csharp
TArg someTArg;
...Select((in T x) => new SomeStruct(in x, someTArg))... // Everytime this line executes, a new capture object for `someTArg` must be allocated on the managed heap.
...Select(static (in T x, TArg a) => new SomeStruct(in x, a), someTArg)... // No capture is happening. ('static' is not mandatory, just a explicit declaration)

```

## Things to do

1. More documentations.
2. Larger test coverage.
3. More collection types.
4. More LINQ providers and support range.
5. Roslyn analyzer for struct lifetime/ownership enforcing. (The actual lifetime is not being enforced, such as the early dispose from the owner side or mutation from the borrower side is still unpreventable, static analysis with attribute markers should be the way to go.)

## Thanks to

* Emma Maassen from <https://github.com/Enichan/Arenas>

* Angouri from <https://github.com/asc-community/HonkPerf.NET>

Details in [THIRD-PARTY-NOTICES.md](https://github.com/fryderykhuang/NullGC/blob/main/THIRD-PARTY-NOTICES.md)

## How to contribute

Framework projects like this will not become generally useful without being battle tested in real world. If your project can protentially benefit from this library, feel free to submit an Issue and talk about your use case. Any type of contributions are welcomed.
