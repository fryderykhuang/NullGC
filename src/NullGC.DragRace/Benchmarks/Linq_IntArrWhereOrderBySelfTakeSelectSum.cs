using BenchmarkDotNet.Attributes;
using Cathei.LinqGen;
using NetFabric.Hyperlinq;
using NullGC.Linq;

namespace NullGC.DragRace.Benchmarks;

[PickedForCicd]
public class Linq_IntArrWhereOrderBySelfTakeSelectWithArgSum : LinqBenchmarkBase
{
    [Benchmark]
    public void NullGCLinqRef_IntArr()
    {
        var f = 1.5f;
        _dummyFloat = _intArr.LinqRef().Where((in int x) => x > 100).Order().Take(500)
            .Select((in int x, float a) => x * a, f).Sum();
    }

    [Benchmark]
    public void NullGCLinqRef_ValIntArr()
    {
        var f = 1.5f;
        _dummyFloat = _valIntArr.LinqRef().Where((in int x) => x > 100).Order().Take(500)
            .Select((in int x, float a) => x * a, f)
            .Sum();
    }

    [Benchmark]
    public void NullGCLinqValue_ValIntArr()
    {
        var f = 1.5f;
        _dummyFloat = _valIntArr.LinqValue().Where(x => x > 100).Order().Take(500).Select((x, a) => x * a, f)
            .Sum();
    }
    
    [Benchmark]
    public void LinqGen_IntArr_Capture()
    {
        var f = 1.5f;
        _dummyFloat = _intArr.Gen().Where(x => x > 100).OrderBy(x=>x).Take(500).Select(x => x * f).Sum();
    }

    [Benchmark]
    public void LinqGen_IntArr_NoArg()
    {
        _dummyFloat = _intArr.Gen().Where(x => x > 100).OrderBy(x => x).Take(500).Select(x => x * 1.5f).Sum();
    }
    
    [Benchmark]
    public void HyperLinq_IntArr_NoArg()
    {
        _dummyFloat = _intArr.AsValueEnumerable().Where(x => x > 100).OrderBy(x => x).Take(500).Select(x => x * 1.5f).Sum();
    }
}