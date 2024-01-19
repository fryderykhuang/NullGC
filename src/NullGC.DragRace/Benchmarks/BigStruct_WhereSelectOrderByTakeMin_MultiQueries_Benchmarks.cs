using BenchmarkDotNet.Attributes;
using Cathei.LinqGen;
using NetFabric.Hyperlinq;
using NullGC.DragRace.Models;
using NullGC.Linq;

namespace NullGC.DragRace.Benchmarks;

[PickedForCicd]
public class BigStruct_WhereSelectOrderByTakeMin_MultiQueries_Benchmarks : LinqBenchmarkBase
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
    public void NullGCLinqValue_BigStruct()
    {
        _dummyLong = _bigStructValArr.LinqValue().Where(x => x.Key > 100000)
            .OrderBy(x => x.Key)
            .Take(500).Select(x => x.Key).Min();
        _dummyLong = _bigStructValArr.LinqValue().Where(x => x.Key < 200000)
            .OrderByDescending(x => x.Key)
            .Take(500).Select(x => x.Key).Min();
        _dummyLong = _bigStructValArr.LinqValue().Where(x => x.Key > 100000)
            .OrderBy(x => x.Key)
            .Take(500).Select(x => x.Key).Min();
        _dummyLong = _bigStructValArr.LinqValue().Where(x => x.Key < 200000)
            .OrderByDescending(x => x.Key)
            .Take(500).Select(x => x.Key).Min();
    }
    
    [Benchmark]
    public void NullGCLinqPtr_BigStruct()
    {
        unsafe
        {
            _dummyLong =_bigStructValArr.LinqPtr().Where(x => x->Key > 100000)
                .OrderBy(x => x->Key)
                .Take(500).Select(x => x->Key).Min();
            _dummyLong =_bigStructValArr.LinqPtr().Where(x => x->Key < 200000)
                .OrderByDescending(x => x->Key)
                .Take(500).Select(x => x->Key).Min();
            _dummyLong =_bigStructValArr.LinqPtr().Where(x => x->Key > 100000)
                .OrderBy(x => x->Key)
                .Take(500).Select(x => x->Key).Min();
            _dummyLong =_bigStructValArr.LinqPtr().Where(x => x->Key < 200000)
                .OrderByDescending(x => x->Key)
                .Take(500).Select(x => x->Key).Min();
        }
    }
    
    [Benchmark]
    public void NullGCLinqRef_BigStruct()
    {
        _dummyLong = _bigStructValArr.LinqRef().Where((in BigStruct x) => x.Key > 100000)
            .OrderBy((in BigStruct x) => x.Key)
            .Take(500).Select((in BigStruct x) => x.Key).Min();
        _dummyLong = _bigStructValArr.LinqRef().Where((in BigStruct x) => x.Key < 200000)
            .OrderByDescending((in BigStruct x) => x.Key)
            .Take(500).Select((in BigStruct x) => x.Key).Min();
        _dummyLong = _bigStructValArr.LinqRef().Where((in BigStruct x) => x.Key > 100000)
            .OrderBy((in BigStruct x) => x.Key)
            .Take(500).Select((in BigStruct x) => x.Key).Min();
        _dummyLong = _bigStructValArr.LinqRef().Where((in BigStruct x) => x.Key < 200000)
            .OrderByDescending((in BigStruct x) => x.Key)
            .Take(500).Select((in BigStruct x) => x.Key).Min();
    }
    
    [Benchmark]
    public void SystemLinq_BigStruct()
    {
        _dummyLong = _bigStructArr.Where(x => x.Key > 100000).OrderBy(x => x.Key).Take(500).Select(x => x.Key).Min();
        _dummyLong = _bigStructArr.Where(x => x.Key < 200000).OrderByDescending(x => x.Key).Take(500).Select(x => x.Key)
            .Min();
        _dummyLong = _bigStructArr.Where(x => x.Key > 100000).OrderBy(x => x.Key).Take(500).Select(x => x.Key).Min();
        _dummyLong = _bigStructArr.Where(x => x.Key < 200000).OrderByDescending(x => x.Key).Take(500).Select(x => x.Key)
            .Min();
    }

    [Benchmark]
    public void LinqGen_BigStruct()
    {
        _dummyLong = _bigStructArr.Gen().Where(x => x.Key > 100000).OrderBy(x => x.Key).Take(500).Select(x => x.Key)
            .Min();
        _dummyLong = _bigStructArr.Gen().Where(x => x.Key < 200000).OrderByDescending(x => x.Key).Take(500).Select(x => x.Key)
            .Min();
        _dummyLong = _bigStructArr.Gen().Where(x => x.Key > 100000).OrderBy(x => x.Key).Take(500).Select(x => x.Key)
            .Min();
        _dummyLong = _bigStructArr.Gen().Where(x => x.Key < 200000).OrderByDescending(x => x.Key).Take(500).Select(x => x.Key)
            .Min();
    }
    
    [Benchmark]
    public void HyperLinq_BigStruct()
    {
        _dummyLong = _bigStructArr.AsValueEnumerable().Where(x => x.Key > 100000).OrderBy(x => x.Key).Take(500).Select(x => x.Key)
            .Min();
        _dummyLong = _bigStructArr.AsValueEnumerable().Where(x => x.Key < 200000).OrderByDescending(x => x.Key).Take(500).Select(x => x.Key)
            .Min();
        _dummyLong = _bigStructArr.AsValueEnumerable().Where(x => x.Key > 100000).OrderBy(x => x.Key).Take(500).Select(x => x.Key)
            .Min();
        _dummyLong = _bigStructArr.AsValueEnumerable().Where(x => x.Key < 200000).OrderByDescending(x => x.Key).Take(500).Select(x => x.Key)
            .Min();
    }
}