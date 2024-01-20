using BenchmarkDotNet.Attributes;
using NullGC.Allocators;
using NullGC.Allocators.Extensions;
using NullGC.Collections;

namespace NullGC.DragRace.Benchmarks;

[PickedForCicd]
public class Allocator_IntArrayAllocationOverTimeBenchmarks : BenchmarkBase
{
    private int[] _arr;
    private ValueArray<int> _valIntArr;
    private int _count = 50000;
    private Random _rand;
    private int _i;
    private int _ratio = 20;
    private int _randMax = 100;

    [GlobalSetup]
    public void Setup()
    {
        AllocatorContext.Impl.ConfigureDefaultUnscoped(cacheTtlMs: 100, cacheLostObserveWindowSize: 1,
            cleanupTh: 8 * 1024 * 1024 / 100);
        // AllocatorContext.Impl.ConfigureDefaultUncachedUnscoped();
        _rand = new Random(0);
    }

    [IterationSetup]
    public void IterationSetup()
    {
        Thread.Sleep(5);
    }


    [IterationCount(2000)]
    [InvocationCount(1)]
    [Benchmark]
    public void NewValueInt()
    {
        _valIntArr = new ValueArray<int>(_rand.Next(0, _randMax) * _ratio);
        _valIntArr.Dispose();
    }

    // [InvocationCount(2000)]
    [IterationCount(2000)]
    [InvocationCount(1)]
    [Benchmark]
    public void NewValueIntNoClear()
    {
        _valIntArr = new ValueArray<int>(_rand.Next(0, _randMax) * _ratio, noClear: true);
        _valIntArr.Dispose();
    }
    
    [IterationCount(2000)]
    [InvocationCount(1)]
    [Benchmark]
    public void NewInt()
    {
        _arr = new int[_rand.Next(0, _randMax) * _ratio];
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        AllocatorContext.ClearProvidersAndAllocations();
    }
}