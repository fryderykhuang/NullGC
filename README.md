[![NullGC](https://github.com/fryderykhuang/NullGC/actions/workflows/main.yml/badge.svg)](https://github.com/fryderykhuang/NullGC/actions/workflows/main.yml)
![NuGet Version](https://img.shields.io/nuget/vpre/NullGC.Abstractions?label=NullGC.Abstractions)
![NuGet Version](https://img.shields.io/nuget/vpre/NullGC.Allocators?label=NullGC.Allocators)
![NuGet Version](https://img.shields.io/nuget/vpre/NullGC.Collections?label=NullGC.Collections)
![NuGet Version](https://img.shields.io/nuget/vpre/NullGC.Linq?label=NullGC.Linq)
[![](https://dcbadge.vercel.app/api/server/SQg4eUW63S?style=flat&compact=true)](https://discord.gg/SQg4eUW63S)

# NullGC

High performance unmanaged memory allocator / collection types / LINQ provider for .NET Core.
Most suitable for game development since there will be no latency jitter caused by .NET garbage collection activities.
[Benchmark Results](https://fryderykhuang.github.io/NullGC/) (Auto updated by GitHub Actions)

## Motivation

This project was born mostly out of my curiosity on how far can it go to entirely eliminate garbage collection, also as a side project emerged from an ongoing game engine development. Although .NET background GC is already good at hiding GC stops, still there are some. Also for throughput focused scenarios, there may be huge performance potential when GC is completely out of the equation according to my previous experience on realtime data processing.

## Usage

Currently this project contains 4 main components:

1. Unmanaged memory allocator
2. Value type (struct) only collections
3. Linq operators
4. Struct ownership analyzer

### Setup

1. Install NuGet package `NullGC.Allocators`, `NullGC.Collections` and optionally `NullGC.Linq`
2. Install the `NulGC.Analyzer` NuGet package. (For LINQ operation boxing detection and value type lifetime enforcement, the warning codes produced are prefixed with `NGC`)
****Full struct ownership enforcement via analyzer is on the way*** (Update on 2/7/2024: Partially implemented)
3. Setup AllocatorContext:

```csharp
AllocatorContext.SetImplementation(new DefaultAllocatorContextImpl()); // new DefaultAllocatorContextImpl().ConfigureDefault() will not work, big mistake in previous documentation.
AllocatorContext.Impl.ConfigureDefault();
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

### Explicit ownership management

With the double-free problem discussed above, `IExplicitOwnership<TSelf>` is introduced to tackle the issue regarding vague struct ownership (*The 'ownership' here currently only focuses on who should be dispose the disposable struct, in theory should at least include immutability on the borrowed copy, may extend to rust-like true ownership semantic in the future) when the struct is being copied such as passed by value. The interface is defined as following:

```csharp
public interface IExplicitOwnership<out T> : IDisposable where T : struct, IDisposable
{
    [return: Borrowed]
    T Borrow();

    [return: Owned]
    T Take();
}
```

When a struct needs to be disposable, this interface should be implemented. It contains two methods `Borrow()` and `Take()`. `Borrow` should return a copy of the struct itself with `Dispose()` changed to a no-op. `Take` should also return a copy of itself but instead with the `Dispose()` method of the original struct be changed to a no-op (*See implementation in `ValueArray<T>`). Semantically, the responsibility of calling the `Dispose()` for the 'borrowed' struct is on the original struct, for the 'taken' struct, it is on the returned struct from calling the `Take()`.

With that in mind, to prevent double-free, when the implementer of `IExplicitOwnership<TSelf>` are passed by value, `Borrow()` should be used:

```csharp
void SomeMethod(ValueList<T> lst){ // 'lst' is a copy of 'list' below.
    lst.Dispose(); // Does nothing.
}

var list = new ValueList<T>(AllocatorTypes.DefaultUnscoped);
SomeMethod(list.Borrow()); // The borrowed one's Dispose() is a no-op.
list.Dispose(); // Ok.
```

If another `TSelf` returning method or property getter already have ***borrowed*** or ***owned*** semantic, you can tag the return value with the corresponding attribute, after that, the return value obtained from calling this method will have that kind of explicit ownership. Also, attribute on base type or interface will be inherited on derived types, as you can see, `IExplicitOwnership<TSelf>` is implemented this way.

The `[Borrowed]/[Owned]` attributes are used to tag the return value as a ***borrowed*** (with no-op `Dispose()`) copy or a ***owned*** (same as original) copy, as long as the attribute is defined

**Installing `NullGC.Analyzer` is required to make sure the user code be analyzed according to the ownership semantic discussed above and specific warnings be given out to the developer at IntelliSense and compile time.**

### Interop with managed object

If you have to use managed object (i.e. class) inside a struct, you can use
`Pinned<T>` to pin the object down so that its address is fixed and can be stored on a non-GC rooted place.

*Start from .NET 5 there's a specific heap type for pinned object called [POH](https://devblogs.microsoft.com/dotnet/internals-of-the-poh/), the performance impact will be quite low.

### Linq

**The fastest LINQ provider as of today** (2024.1). [Benchmark Results](https://fryderykhuang.github.io/NullGC/) (compared with Built-in/LinqGen/RefLinq/HyperLinq)

The extreme performance boils down to:

1. Minimize struct copy by aggressive inlining and use ref modifier.
2. No boxing (except for some case of interface casting that cannot be optimized away).
3. Exploit the traits of previous stage as much as possible. (e.g. if the previous of OrderBy is `IAddressFixed`(contains only unmanaged structs rooted on fixed address object or stack, or gc-pinned managed objects), we can store the pointer instead of the whole struct)

Proper usage is with the built-in value typed collections, but good old `IEnumerable<T>` is also supported. You can still get some benefit on LINQ operators that need to buffer data such as OrderBy.
The LINQ interface has 2 variations:

```csharp
SomeCollection.LinqValue()... // Enumerate by value. All types implement IEnumerable<T> are supported
SomeCollection.LinqRef()...   // Enumerate by ref. Besides value collections, only collection with Enumerator implemented `ILinqRefEnumerator<T>` are supported (e.g. normal array types)
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

*For now only a portion of LINQ operators are implemented, since all custom LINQ operator structs also implement `IEnumerable<T>`, if an operator/input type combination is not implemented, the system LINQ extension method will be called instead, which will cause the boxing of all the structs the LINQ chain is composed of.
~Until the corresponding Roslyn analyzer is implemented or some boxing/heap allocation analyzer is used, this situation should be examined carefully.~
**Use `NullGC.Analyzer` to produce warnings on these scenarios.**

## Things to do

1. More documentations.
2. Larger test coverage.
3. More collection types.
4. More LINQ operators, support more input types.
5. Roslyn analyzer for struct lifetime/ownership enforcing. (The actual lifetime is not being enforced, such as the early dispose from the owner side or mutation from the borrower side is still unpreventable, static analysis with attribute markers should be the way to go.) ****Partially implemented.***
6. ~Roslyn analyzer for unintended boxing when using NullGC.Linq~
7. ~Vsix extension for analyzers~ (It seems digital signing is required for the extension to be updatable, stay away from the mess for now)

## Thanks to

* Emma Maassen from <https://github.com/Enichan/Arenas>

* Angouri from <https://github.com/asc-community/HonkPerf.NET>

Details in [THIRD-PARTY-NOTICES.md](https://github.com/fryderykhuang/NullGC/blob/main/THIRD-PARTY-NOTICES.md)

## How to contribute

Project like this will not become generally useful without being battle tested in real world. If your project can potentially benefit from this library, feel free to talk about your use case in the [Discussions](https://github.com/fryderykhuang/NullGC/discussions) area. Any form of contributions are welcomed.
