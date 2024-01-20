using BenchmarkDotNet.Attributes;
using NullGC.Allocators;
using NullGC.Allocators.Extensions;
using NullGC.Collections;
using Xunit.Sdk;

namespace NullGC.DragRace.Benchmarks;


[PickedForCicd]
public class Allocator_IntListGrowingBenchmarks : BenchmarkBase
{
    public Allocator_IntListGrowingBenchmarks()
    {
    }
    
    private List<int> _intList = null!;
    // ReSharper disable once CollectionNeverQueried.Local
    private ValueList<int> _valIntList;
    private int _count = 300_000_000;

    [GlobalSetup]
    public void Setup()
    {
        AllocatorContext.Impl.ConfigureDefaultUnscoped();
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _valIntList = new ValueList<int>();
        _intList = new List<int>();
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
        _valIntList.Dispose();
        _intList = null!;
        if (AllocatorContext.GetAllocator() is IAllocatorCacheable c)
            c.ClearCachedMemory();
    }
    
    [InvocationCount(1)]
    [Benchmark]
    public void ValueList()
    {
        for (int i = 0; i < _count; i++)
        {
            _valIntList.Add(i);
        }
    }

    [InvocationCount(1)]
    [Benchmark]
    public void SystemList()
    {
        for (int i = 0; i < _count; i++)
        {
            _intList.Add(i);
        }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        AllocatorContext.ClearProvidersAndAllocations();
    }
}