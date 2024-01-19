using BenchmarkDotNet.Attributes;
using Cathei.LinqGen;
using NullGC.Linq;

namespace NullGC.DragRace.Benchmarks;

[PickedForCicd]
public class IntArrWhereOrderBySelfTakeSelectSum : LinqBenchmarkBase
{
    [Benchmark]
    public void NullGCLinqRef_IntArr()
    {
        _dummyFloat = _intArr.LinqRef().Where((in int x) => x > 100).OrderBy().Take(500).Select((in int x, float a) => x * a, 1.5f).Sum();
    }
    
    [Benchmark]
    public void NullGCLinqRef_ValIntArr()
    {
        _dummyFloat = _valIntArr.LinqRef().Where((in int x) => x > 100).OrderBy().Take(500).Select((in int x, float a) => x * a, 1.5f)
            .Sum();
    }

    [Benchmark]
    public void NullGCLinqValue_ValIntArr()
    {
        _dummyFloat = _valIntArr.LinqValue().Where(x => x > 100).OrderBy().Take(500).Select((x, a) => x * a, 1.5f).Sum();
    }
    //
    // [Benchmark]
    // public void LinqGen_IntArr_Capture()
    // {
    //     var a = 1.5f;
    //     _dummyFloat = _intArr.Gen().Where(x => x > 100).OrderBy(x=>x).Take(500).Select(x => x * a).Sum();
    // }
    
    // [Benchmark]
    // public void LinqGen_IntArr()
    // {
    //     _dummyFloat = _intArr.Gen().Where(x => x > 100).OrderBy(x => x).Take(500).Select(x => x * 1.5f).Sum();
    // }
}