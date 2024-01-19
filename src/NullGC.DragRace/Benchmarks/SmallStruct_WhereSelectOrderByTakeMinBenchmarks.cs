using BenchmarkDotNet.Attributes;
using Cathei.LinqGen;
using NetFabric.Hyperlinq;
using NullGC.DragRace.Models;
using NullGC.Linq;

namespace NullGC.DragRace.Benchmarks;

[PickedForCicd]
public class SmallStruct_WhereSelectOrderByTakeMinBenchmarks : LinqBenchmarkBase
{
    //
    // [Benchmark]
    // public void RefLinqBigStructValArrWhereSelectSum()
    // {
    //     _sumLong = _bigStructArr.ToRefLinq().Where(x => x.Key > 100)
    //         .Select(x => x.Value)
    //         .Sum();
    // }

    // [Benchmark]
    // public void BigStructValArrNullGCLinqWhereSelectSum()
    // {
    //     _sumLong = _bigStructValArr.LinqRef().Where((in BigStruct x) => x.Key > 100)
    //         .Select((ref BigStruct x) => ref x.Value)
    //         .Sum();
    // }
    //

    //
    // [Benchmark]
    // public void ValIntArrNullGCPiperRefWhereSum()
    // {
    //     _sumLong = _valIntArr.AsRefSuckable().Where((in int x) => x > 100)
    //         .Sum();
    // }
    //
    // [Benchmark]
    // public void ValIntArrNullGCPiperValueWhereSum()
    // {
    //     _sumLong = _valIntArr.AsValueSuckable().Where(x => x > 100)
    //         .Sum();
    // }
    //
    // [Benchmark]
    // public void IntArrRefLinqWhereSum()
    // {
    //     _sumLong = _intArr.ToRefLinq().Where(x => x > 100)
    //         .Sum();
    // }

    // [Benchmark]
    // public void BigStructValArrNullGCLinqRefWhereOrderByTakeAverage()
    // {
    //     _double = _bigStructValArr.LinqRef().Where((in BigStruct x) => x.Key > 0)
    //         .OrderBy((in BigStruct x) => x.Key)
    //         .Take(500).Select((in BigStruct x) => x.Key).Average();
    // }
    //


    [Benchmark]
    public void NullGCLinqValue_SmallStruct()
    {
        _dummyLong = _smallStructValArr.LinqValue().Where(x => x.Key > 100000)
            .OrderBy(x => x.Key)
            .Take(500).Select(x => x.Key).Min();
    }

    [Benchmark]
    public void NullGCLinqRef_SmallStruct()
    {
        _dummyLong = _smallStructValArr.LinqRef().Where((in SmallStruct x) => x.Key > 100000)
            .OrderBy((in SmallStruct x) => x.Key)
            .Take(500).Select((in SmallStruct x) => x.Key).Min();
    }

    [Benchmark]
    public void SystemLinq_SmallStruct()
    {
        _dummyLong = _smallStructArr.Where(x => x.Key > 100000).OrderBy(x => x.Key).Take(500).Select(x => x.Key).Min();
    }

    [Benchmark]
    public void LinqGen_SmallStruct()
    {
        _dummyLong = _smallStructArr.Gen().Where(x => x.Key > 100000).OrderBy(x => x.Key).Take(500).Select(x => x.Key)
            .Min();
    }

    [Benchmark]
    public void HyperLinq_SmallStruct()
    {
        _dummyLong = _smallStructArr.AsValueEnumerable().Where(x => x.Key > 100000).OrderBy(x => x.Key).Take(500)
            .Select(x => x.Key)
            .Min();
    }
}