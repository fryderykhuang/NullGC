using BenchmarkDotNet.Attributes;
using NullGC.Allocators;
using NullGC.Allocators.Extensions;
using NullGC.Collections;

namespace NullGC.DragRace.Benchmarks;

[PickedForCicd]
public class Allocator_IntArrayAllocationBenchmarks : BenchmarkBase
{
    private int[] _arr;
    private ValueArray<int> _valIntArr;
    private int _count = 50000;
    private Random _rand;
    private int _i;
    private int _ratio = 20;
    private int _randMax = 5000;

    [GlobalSetup]
    public void Setup()
    {
        AllocatorContext.Impl.ConfigureDefaultUnscoped();
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _rand = new Random(0);
    }


    [InvocationCount(1)]
    [Benchmark]
    public void NewValueInt()
    {
        for (int i = 0; i < _count; i++)
        {
            _valIntArr = new ValueArray<int>(_rand.Next(0, _randMax) * _ratio);
            _valIntArr.Dispose();
        }
    }

    [InvocationCount(1)]
    [Benchmark]
    public void NewValueIntNoClear()
    {
        for (int i = 0; i < _count; i++)
        {
            _valIntArr = new ValueArray<int>(_rand.Next(0, _randMax) * _ratio, noClear: true);
            _valIntArr.Dispose();
        }
    }

    [InvocationCount(1)]
    [Benchmark]
    public void NewInt()
    {
        for (int i = 0; i < _count; i++)
        {
            _arr = new int[_rand.Next(0, _randMax) * _ratio];
        }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        AllocatorContext.ClearProvidersAndAllocations();
    }
}