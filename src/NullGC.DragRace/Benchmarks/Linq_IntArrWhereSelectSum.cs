using BenchmarkDotNet.Attributes;
using Cathei.LinqGen;
using HonkPerf.NET.RefLinq;
using NullGC.Linq;

namespace NullGC.DragRace.Benchmarks;

public class Linq_IntArrWhereSelectSum : LinqBenchmarkBase
{
    [Benchmark]
    public void NullGCLinqRef_IntArr()
    {
        _dummyFloat = _intArr.LinqRef().Where((in int x) => x > 100).Select((in int x, float a) => x * a, 1.5f).Sum();
    }

    [Benchmark]
    public void NullGCLinqRef_ValIntArr()
    {
        _dummyFloat = _valIntArr.LinqRef().Where((in int x) => x > 100).Select((in int x, float a) => x * a, 1.5f)
            .Sum();
    }

    [Benchmark]
    public void NullGCLinqValue_ValIntArr()
    {
        _dummyFloat = _valIntArr.LinqValue().Where(x => x > 100).Select((x, a) => x * a, 1.5f).Sum();
    }

    [Benchmark]
    public void RefLinq_IntArr()
    {
        _dummyFloat = _intArr.ToRefLinq().Where(x => x > 100).Select((x, a) => x * a, 1.5f).Sum();
    }

    [Benchmark]
    public void LinqGen_IntArr_Capture()
    {
        var a = 1.5f;
        _dummyFloat = _intArr.Gen().Where(x => x > 100).Select(x => x * a).Sum();
    }
    
    [Benchmark]
    public void LinqGen_IntArr()
    {
        _dummyFloat = _intArr.Gen().Where(x => x > 100).Select(x => x * 1.5f).Sum();
    }
}