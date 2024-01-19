using BenchmarkDotNet.Attributes;
using Cathei.LinqGen;
using HonkPerf.NET.RefLinq;
using NullGC.Allocators;
using NullGC.Allocators.Extensions;
using NullGC.Collections;
using NullGC.DragRace.Models;
using NullGC.Linq.Enumerators;
using NullGC.TestCommons;

namespace NullGC.DragRace;


// [Config(typeof(FastConfig))]

public abstract class LinqBenchmarkBase : BenchmarkBase
{
    protected int[] _intArr;
    protected ValueArray<int> _valIntArr;
    protected BigStructGeneric<int, float>[] _bigStructArr;
    protected SmallStructGeneric<int, float>[] _smallStructArr;
    protected ValueArray<BigStructGeneric<int, float>> _bigStructGenValArr;
    protected ValueArray<SmallStructGeneric<int, float>> _smallStructGenValArr;
    protected ValueArray<BigStruct> _bigStructValArr;
    protected ValueArray<SmallStruct> _smallStructValArr;
    private const int _count = 50000;

    [GlobalSetup]
    public void Setup()
    { 
        AllocatorContext.Impl.ConfigureDefaultUnscoped();
        _intArr = new int[_count];
        _valIntArr = new ValueArray<int>(_count);
        _bigStructArr = new BigStructGeneric<int, float>[_count];
        _smallStructArr = new SmallStructGeneric<int, float>[_count];
        _bigStructValArr = new ValueArray<BigStruct>(_count);
        _smallStructValArr = new ValueArray<SmallStruct>(_count);
        _bigStructGenValArr = new ValueArray<BigStructGeneric<int, float>>(_count);
        _smallStructGenValArr = new ValueArray<SmallStructGeneric<int, float>>(_count);

        RandomFiller.FillArrayRandom<int, int[]>(_intArr);
        RandomFiller.FillArrayRandom<int, ValueArray<int>>(_valIntArr);
        RandomFiller.FillArrayRandom(_bigStructArr, (ref BigStructGeneric<int, float> t1) => ref t1.Key);
        RandomFiller.FillArrayRandom(_smallStructArr, (ref SmallStructGeneric<int, float> t1) => ref t1.Key);
        RandomFiller.FillArrayRandom(_bigStructValArr, (ref BigStruct t1) => ref t1.Key);
        RandomFiller.FillArrayRandom(_smallStructValArr, (ref SmallStruct t1) => ref t1.Key);
        RandomFiller.FillArrayRandom(_bigStructGenValArr, (ref BigStructGeneric<int, float> t1) => ref t1.Key);
        RandomFiller.FillArrayRandom(_smallStructGenValArr, (ref SmallStructGeneric<int, float> t1) => ref t1.Key);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        AllocatorContext.ClearProvidersAndAllocations();
    }
    
    protected static int _dummyInt;
    protected static float _dummyFloat;
    protected static double _dummyDouble;
    protected static long _dummyLong;
}
