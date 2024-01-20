[![NullGC](https://github.com/fryderykhuang/NullGC/actions/workflows/main.yml/badge.svg)](https://github.com/fryderykhuang/NullGC/actions/workflows/main.yml)
![NuGet Version](https://img.shields.io/nuget/vpre/NullGC.Abstractions?label=NullGC.Abstractions)
![NuGet Version](https://img.shields.io/nuget/vpre/NullGC.Allocators?label=NullGC.Allocators)
![NuGet Version](https://img.shields.io/nuget/vpre/NullGC.Collections?label=NullGC.Collections)
![NuGet Version](https://img.shields.io/nuget/vpre/NullGC.Linq?label=NullGC.Linq)
# NullGC
High performance unmanaged memory allocator / collection types / LINQ provider for .NET Core.
Most suitable for game development since there will be no latency jitter caused by .NET garbage collection activities.

## Motivation
This project was born mostly out of my curiosity on how far can it go to entirely eliminate garbage collection. Although .NET background GC is already good at hiding GC stops, still there are some. Also for throughput focused scenarios, there may be huge throughput difference when GC is completely out of the equation.

## Usage
Currently this project contains 3 components: 
1. Unmanaged memory allocator
2. Value type only collections
3. Linq operators

Two types of memory allocation strategy are supported.
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

[Benchmark Results Here](https://fryderykhuang.github.io/NullGC/) (Auto updated by GitHub Actions, compared with LinqGen/RefLinq/HyperLinq)

Proper usage is with the built-in value typed collections, but good old IEnumerable&lt;T&gt; is also supported. You can still get some benefit on LINQ operators that need to buffer data such as OrderBy.
The LINQ interface has 3 variations:
```csharp
SomeCollection.LinqValue()... // All types of IEnumerable<T> are supported
SomeCollection.LinqRef()...   // Besides built-in value typed collections, only Enumerators that exposes 'ref T Current' are supported (e.g. normal array types)
SomeCollection.LinqPtr()...   // Only built-in value typed collections are supported. 
```

## Things to do

1. More examples.
2. Larger test coverage.
3. More collection types.
4. More LINQ providers and support range.

## Thanks

Many thanks to Emma Maassen from <https://github.com/Enichan/Arenas> and Angouri from <https://github.com/asc-community/HonkPerf.NET> on inspiring me to this project.

Details in [THIRD-PARTY-NOTICES.md](https://github.com/fryderykhuang/NullGC/blob/main/THIRD-PARTY-NOTICES.md)