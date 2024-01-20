[![NullGC](https://github.com/fryderykhuang/NullGC/actions/workflows/main.yml/badge.svg)](https://github.com/fryderykhuang/NullGC/actions/workflows/main.yml)
![NuGet Version](https://img.shields.io/nuget/vpre/NullGC.Abstractions?label=NullGC.Abstractions)
![NuGet Version](https://img.shields.io/nuget/vpre/NullGC.Allocators?label=NullGC.Allocators)
![NuGet Version](https://img.shields.io/nuget/vpre/NullGC.Collections?label=NullGC.Collections)
![NuGet Version](https://img.shields.io/nuget/vpre/NullGC.Linq?label=NullGC.Linq)

# NullGC

High performance unmanaged memory allocator / collection types / LINQ provider for .NET Core. ([Benchmark Results](https://fryderykhuang.github.io/NullGC/))
Most suitable for game development since there will be no latency jitter caused by .NET garbage collection activities.

## Motivation

This project was born mostly out of my curiosity on how far can it go to entirely eliminate garbage collection. Although .NET background GC is already good at hiding GC stops, still there are some. Also for throughput focused scenarios, there may be huge throughput difference when GC is completely out of the equation.

## Usage

Currently this project contains 3 components:

1. Unmanaged memory allocator
2. Value type (struct) only collections
3. Linq operators

### Setup

Use this line to setup the AllocatorContext, which is used internally in `ValueArray<T>` and any other locations that need to allocate unmanaged memory.

```csharp
        AllocatorContext.SetImplementation(new DefaultAllocatorContextImpl().ConfigureDefault());
```

### Memory allocation strategies

Two types of memory allocation strategy are supported:

#### 1. Arena

```csharp
// all 'T' is struct if not specifically mentioned.
using (AllocatorContext.BeginAllocationScope())
{
    var list = new ValueList<T>(); // plain old 'new'
    var dict = new ValueDictionary<T>();
    var obj = new Allocated<T>(); // let struct works like a class (struct is allocated on the unmanaged heap.)
    ...
} // all value collections are automatically disposed as they go out of scope, no need to explicitly call Dispose().
```

#### 2. Explicit lifetime

You can utilize the unmanaged allocator anywhere like this (including inside of arena scope):

```csharp
// use the overload with parameter 'AllocatorTypes', then specify the unscoped, globally available allocator type.
var list = new ValueList<T>(AllocatorTypes.DefaultUnscoped); 
var obj = new Allocated<T>(AllocatorTypes.DefaultUnscoped);
...
// Anywhere after usage:
list.Dispose();
```

#### Pass by ref or by value

Since we are using struct everywhere, how to pass a struct which works like a reference type is a little bit tricky.

Under most circumstances, use `ref` modifier will be sufficient, but there's still somewhere that cannot use the `ref` modifier such as struct field (`ref` type can only be put in a `ref struct`, which is incovienient).

To avoid double-free, when those value collections are passed by value, Borrow() should be used. After calling Borrow(), all copies of the original collection can be safely disposed without double-free.

```csharp
var list = new ValueList<T>(AllocatorTypes.DefaultUnscoped);
...
// ref passing is not affected.
SomeListRefConsumingMethod(in list);
SomeListRefConsumingMethod(ref list);

// value passing should call Borrow() unless you're certain the passed one will not be disposed.
SomeListConsumingMethod(list.Borrow())
```

#### 3. Interop with managed object

if you have to use managed object(classes) inside a struct, you can use
`Pinned<T>` to pin the object down so that its address is fixed and can be stored on a non-GC rooted place.


### Custom collection types

* ValueArray&lt;T&gt;
* ValueList&lt;T&gt;
* ValueDictionary&lt;TKey, TValue&gt;
* ValueStack&lt;T&gt;
* ValueLinkedList&lt;T&gt;
* ValueFixedSizeDeque&lt;T&gt; (Circular buffer)
* SlidingWindow&lt;T&gt;
* SlidingTimeWindow&lt;T&gt;

All collection types can be enumerated by ref (`foreach(ref var item in collection)`)

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
