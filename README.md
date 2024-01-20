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

Two types of memory allocation strategy are supported:

### Arena

```csharp
using (AllocatorContext.BeginAllocationScope())
{
    var list = new ValueList<T>();
    var dict = new ValueDictionary<T>();
    ...
} // all Value* collections are automatically disposed as they go out of scope. 
```

### Explicit lifetime

```csharp
// You can construct a value type collection anywhere(including inside of arena scope) 
// using this overload:
var list = new ValueList<T>(AllocatorTypes.DefaultUnscoped);
...
// Anywhere after sometime:
list.Dispose();
```

To avoid double-free situations, when these collections are passed by value, Borrow() should be used. After this, all copies of the original collections can be safely disposed without double-free.

```csharp
var list = new ValueList<T>(AllocatorTypes.DefaultUnscoped);
...
// ref passing is not affected.
SomeListRefConsumingMethod(in list);
SomeListRefConsumingMethod(ref list);
// value passing should call Borrow()
SomeListConsumingMethod(list.Borrow())
```

### How to com

### Custom collection types

* ValueArray&lt;T&gt;
* ValueList&lt;T&gt;
* ValueDictionary&lt;TKey, TValue&gt;
* ValueStack&lt;T&gt;
* ValueLinkedList&lt;T&gt;
* ValueFixedSizeDeque&lt;T&gt; (Circular buffer)
* SlidingWindow&lt;T&gt;
* SlidingTimeWindow&lt;T&gt;

### Linq

**The fastest LINQ provider as of today** (2024.1).

[Benchmark Results](https://fryderykhuang.github.io/NullGC/) (Auto updated by GitHub Actions, compared with LinqGen/RefLinq/HyperLinq)

Proper usage is with the built-in value typed collections, but good old IEnumerable&lt;T&gt; is also supported. You can still get some benefit on LINQ operators that need to buffer data such as OrderBy.
The LINQ interface has 3 variations:

```csharp
SomeCollection.LinqValue()... // Enumerate by value. All types implement IEnumerable<T> are supported
SomeCollection.LinqRef()...   // Enumerate by ref. Besides built-in value typed collections, only Enumerators that exposes 'ref T Current' are supported (e.g. normal array types)
SomeCollection.LinqPtr()...   // Enumerate by pointer. Only built-in value typed collections are supported. 
```

Most extension methods that needs a delegate type parameter has an overloads with `in` or `ref` modifier to avoid copying too much data if the Linqed type is a big struct.

```csharp
// T is something BIG.
...Where((in T x) => ...).Select((ref T x) => ref x.SomeField)... // all reference, no copy of T
```

Most extension methods has overloads with a `TArg arg` parameter to avoid unnecessary variable capture, thus avoids the allocation of a capture object.

```csharp
TArg someTArg;
.LinqRef()...Select((in T x) => new SomeStruct(in x, someTArg))... // Everytime this line executes, a new capture object for `someTArg` must be allocated .
.LinqRef()...Select(static (in T x, TArg a) => new SomeStruct(in x, a), someTArg)... // No capture is happening.

```

## Things to do

1. More examples.
2. Larger test coverage.
3. More collection types.
4. More LINQ providers and support range.

## Thanks to

Emma Maassen from <https://github.com/Enichan/Arenas>
Angouri from <https://github.com/asc-community/HonkPerf.NET>

Details in [THIRD-PARTY-NOTICES.md](https://github.com/fryderykhuang/NullGC/blob/main/THIRD-PARTY-NOTICES.md)

## How to contribute

These framework-like projects will not become generally useful without being battle tested in real world. If your project can protentially benefit from this, feel free to submit an Issue and talk about your use case. Any type of contributions are welcomed.
