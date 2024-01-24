using Cathei.LinqGen;
using NullGC.Collections;
using NullGC.Linq.Enumerators;
using NullGC.TestCommons;
using Xunit.Abstractions;

namespace NullGC.Linq.Tests;

public class LinqTests : AssertMemoryAllFreedBase
{
    private const int _count = 2000;
    private readonly int _bigStructMin;
    private readonly ValueArray<int> _emptyArray;
    private readonly int _smallStructMin;
    private BigStruct<int, float>[] _bigStructArr;
    private ValueArray<BigStruct<int, float>> _bigStructValArr;
    private int[] _intArr;
    private SmallStruct<int, float>[] _smallStructArr;
    private ValueArray<SmallStruct<int, float>> _smallStructValArr;
    private ValueArray<int> _valIntArr;
    private ValueList<int> _valList1;

    public LinqTests(ITestOutputHelper logger) : base(logger, false)
    {
        _emptyArray = ValueArray<int>.Empty;
        _valList1 = new ValueList<int>(0) {7, 0, 4, 5, 6, 1, 2, 3, 8, 9};
        _intArr = new int[_count];
        _valIntArr = new ValueArray<int>(_count);
        _bigStructArr = new BigStruct<int, float>[_count];
        _smallStructArr = new SmallStruct<int, float>[_count];
        _bigStructValArr = new ValueArray<BigStruct<int, float>>(_count);
        _smallStructValArr = new ValueArray<SmallStruct<int, float>>(_count);

        RandomFiller.FillArrayRandom<int, int[]>(_intArr);
        RandomFiller.FillArrayRandom<int, ValueArray<int>>(_valIntArr);
        RandomFiller.FillArrayRandom(_bigStructArr, (ref BigStruct<int, float> t1) => ref t1.Key);
        RandomFiller.FillArrayRandom(_smallStructArr, (ref SmallStruct<int, float> t1) => ref t1.Key);
        RandomFiller.FillArrayRandom(_bigStructValArr, (ref BigStruct<int, float> t1) => ref t1.Key);
        RandomFiller.FillArrayRandom(_smallStructValArr, (ref SmallStruct<int, float> t1) => ref t1.Key);
        if (!_intArr.SequenceEqual(_valIntArr))
            throw new InvalidOperationException();
        if (!_bigStructArr.SequenceEqual(_bigStructValArr))
            throw new InvalidOperationException();
        if (!_smallStructArr.SequenceEqual(_smallStructValArr))
            throw new InvalidOperationException();

        _bigStructMin = BigStructArrSystemLinqWhereOrderByTakeAverage();
        _smallStructMin = SmallStructArrSystemLinqWhereOrderByTakeAverage();
    }

    public override void Dispose()
    {
        _bigStructValArr.Dispose();
        _valList1.Dispose();
        _valIntArr.Dispose();
        _smallStructValArr.Dispose();
        base.Dispose();
    }

    [Fact]
    public void WhereOnEmptyArray()
    {
        Assert.Empty(_emptyArray.LinqRef().Where((in int x) => true));
    }

    [Fact]
    public void WhereWithArgs()
    {
        Assert.Equal(new[] {7, 6, 8, 9}, _valList1.LinqRef().Where((in int x, int y) => x > y, 5));
    }

    [Fact]
    public void Where()
    {
        Assert.Equal(new[] {7, 6, 8, 9}, _valList1.LinqRef().Where((in int x) => x > 5));
    }

    [Fact]
    public void WhereSelect()
    {
        Assert.Equal(new float[] {7 * 0.5f, 6 * 0.5f, 8 * 0.5f, 9 * 0.5f},
            _valList1.LinqRef().Where((in int x) => x > 5).Select((in int x, float y) => x * y, 0.5f));
    }

    [Fact]
    public void OrderBy_LinqRef_In()
    {
        Assert.Equal(new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9},
            _valList1.LinqRef().OrderBy((in int x) => x));
    }

    [Fact]
    public void OrderByDesc_LinqRef_In()
    {
        Assert.Equal(new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}.Reverse(),
            _valList1.LinqRef().OrderByDescending((in int x) => x));
    }

    [Fact]
    public void OrderBy_LinqRef_In_Comparer()
    {
        Assert.Equal(new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}.Reverse(),
            _valList1.LinqRef().OrderBy((in int x) => x, (a, b) => b - a));
    }

    [Fact]
    public void OrderByDesc_LinqRef_In_Comparer()
    {
        Assert.Equal(new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9},
            _valList1.LinqRef().OrderByDescending((in int x) => x, (a, b) => b - a));
    }

    [Fact]
    public void OrderBy_LinqRef_Arg()
    {
        Assert.Equal(new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}.Reverse(),
            _valList1.LinqValue().OrderBy((x, a) => x * a, -1));
    }

    [Fact]
    public void OrderByDesc_LinqRef_Arg()
    {
        Assert.Equal(new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9},
            _valList1.LinqValue().OrderByDescending((x, a) => x * a, -1));
    }

    [Fact]
    public void OrderBy_LinqRef_In_Arg()
    {
        Assert.Equal(new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}.Reverse(),
            _valList1.LinqRef().OrderBy((in int x, int a) => x * a, -1));
    }

    [Fact]
    public void OrderByDesc_LinqRef_In_Arg()
    {
        Assert.Equal(new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9},
            _valList1.LinqRef().OrderByDescending((in int x, int a) => x * a, -1));
    }

    [Fact]
    public void OrderByMulti_LinqRef_SingleSorter()
    {
        Assert.Equal(new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9},
            _valList1.LinqRef().OrderBy(OrderBy.Ascending((in int x) => x)));
    }

    [Fact]
    public void OrderByMulti_LinqRef_MultiSorter()
    {
        Assert.Equal(new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9},
            _valList1.LinqRef().OrderBy(OrderBy.Ascending((in int x) => x, OrderBy.Descending((in int x) => x))));
    }

    // [Fact]
    // public void OrderBy_LinqRef_Ref_Throw()
    // {
    //     Assert.Throws<InvalidOperationException>(() => _intList.LinqRef().OrderBy((ref int x) => ref x).ToArray());
    // }
    //
    // [Fact]
    // public void OrderByDesc_LinqRef_Ref_Throw()
    // {
    //     Assert.Throws<InvalidOperationException>(() => _intList.LinqRef().OrderByDe((ref int x) => ref x).ToArray());
    // }

    [Fact]
    public void OrderByMultipleEnumeration()
    {
        var linq = _valList1.LinqRef().OrderBy((in int x) => x);

        Assert.Equal(new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}, linq);
        Assert.Equal(new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}, linq);
        Assert.Equal(new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}, linq);
    }

    [Fact]
    public void Take()
    {
        Assert.Equal(new int[] {0, 1, 2, 3}, _valList1.LinqRef().OrderBy((in int x) => x).Take(4));
    }

    [Fact]
    public void TakeZero()
    {
        Assert.Empty(_valList1.LinqRef().OrderBy((in int x) => x).Take(0));
    }

    [Fact]
    public void TakeMinus()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            _valList1.LinqRef().OrderBy((in int x) => x).Take(-10));
    }

    [Fact]
    public void TakeTooMuch()
    {
        Assert.Equal(new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9},
            _valList1.LinqRef().OrderBy((in int x) => x).Take(100));
    }

    [Fact]
    public void Skip()
    {
        Assert.Equal(new int[] {4, 5, 6, 7, 8, 9}, _valList1.LinqRef().OrderBy((in int x) => x).Skip(4));
    }

    [Fact]
    public void BigStructValArrNullGCLinqRefWhereOrderByTakeAverageValue()
    {
        Assert.Equal(_smallStructMin, _bigStructValArr.LinqValue().Where(x => x.Key > 100000)
            .OrderBy(x => x.Key)
            .Take(500).Select(x => x.Key).Min());
    }

    [Fact]
    public void SmallStructValArrNullGCLinqRefWhereOrderByTakeAverageValue()
    {
        Assert.Equal(_smallStructMin, _smallStructValArr.LinqValue().Where(x => x.Key > 100000)
            .OrderBy(x => x.Key)
            .Take(500).Select(x => x.Key).Min());
    }

    [Fact]
    public void BigStructValArrNullGCLinqRefWhereOrderByTakeAveragePtr()
    {
        unsafe
        {
            Assert.Equal(_bigStructMin, _bigStructValArr.LinqPtr().Where(x => x->Key > 100000)
                .OrderBy(x => x->Key)
                .Take(500).Select(x => x->Key).Min());
        }
    }

    [Fact]
    public void BigStructValArrNullGCLinqRefWhereOrderByTakeAverageRef()
    {
        Assert.Equal(_bigStructMin, _bigStructValArr.LinqRef().Where((in BigStruct<int, float> x) => x.Key > 100000)
            .OrderBy((in BigStruct<int, float> x) => x.Key)
            .Take(500).Select((in BigStruct<int, float> x) => x.Key).Min());
    }

    [Fact]
    public void SmallStructValArrNullGCLinqRefWhereOrderByTakeAverageRef()
    {
        Assert.Equal(_smallStructMin, _smallStructValArr.LinqRef()
            .Where((in SmallStruct<int, float> x) => x.Key > 100000)
            .OrderBy((in SmallStruct<int, float> x) => x.Key)
            .Take(500).Select((in SmallStruct<int, float> x) => x.Key).Min());
    }

    public int BigStructArrSystemLinqWhereOrderByTakeAverage()
    {
        return _bigStructArr.Where(x => x.Key > 100000).OrderBy(x => x.Key).Take(500).Select(x => x.Key).Min();
    }

    public int SmallStructArrSystemLinqWhereOrderByTakeAverage()
    {
        return _smallStructArr.Where(x => x.Key > 100000).OrderBy(x => x.Key).Take(500).Select(x => x.Key).Min();
    }


    [Fact]
    public void BigStructArrLinqGenWhereOrderByTakeAverage()
    {
        Assert.Equal(_bigStructMin,
            _bigStructArr.Gen().Where(x => x.Key > 100000).OrderBy(x => x.Key).Take(500).Select(x => x.Key).Min());
    }

    [Fact]
    public void FirstLastNoPredicateFacts()
    {
        Assert.Equal(7, _valList1.LinqRef().First());
        Assert.Equal(7, _valList1.LinqRef().FirstOrNullRef());
        Assert.Equal(7, _valList1.LinqRef().FirstOrDefault());
        Assert.Equal(7, _valList1.LinqValue().First());
        Assert.Equal(7, _valList1.LinqValue().FirstOrDefault());
        Assert.Equal(7, _valList1.LinqValue().First());
        Assert.Equal(9, _valList1.LinqRef().Last());
        Assert.Equal(9, _valList1.LinqRef().LastOrDefault());
        Assert.Equal(9, _valList1.LinqValue().Last());
        Assert.Equal(9, _valList1.LinqValue().LastOrDefault());
    }

    [Fact]
    public void FirstLastWithPredicateFacts()
    {
        Assert.Equal(8, _valList1.LinqRef().First((in int x) => x > 7));
        Assert.Equal(8, _valList1.LinqRef().FirstOrNullRef((in int x) => x > 7));
        Assert.Equal(8, _valList1.LinqRef().FirstOrDefault((in int x) => x > 7));
        Assert.Equal(8, _valList1.LinqValue().First(x => x > 7));
        Assert.Equal(8, _valList1.LinqValue().FirstOrDefault(x => x > 7));
        Assert.Equal(8, _valList1.LinqValue().First(x => x > 7));
        Assert.Equal(3, _valList1.LinqRef().Last((in int x) => x < 4));
        Assert.Equal(3, _valList1.LinqRef().LastOrDefault((in int x) => x < 4));
        Assert.Equal(3, _valList1.LinqValue().Last(x => x < 4));
        Assert.Equal(3, _valList1.LinqValue().LastOrDefault(x => x < 4));
    }

    [Fact]
    public void ValueFixedSizeDequeEnumeratorFacts()
    {
        using var q = new ValueFixedSizeDeque<int>(7) {1, 2, 3, 4, 5, 6, 7};
        Assert.Equal(new[] { 3,4,5 }, q.LinqValue().Skip(2).Take(3));
        Assert.Equal(new[] { 3 }, q.LinqValue().Take(3).Skip(2));
        Assert.Empty( q.LinqValue().Skip(2).Take(0));
        Assert.Empty(q.LinqValue().Take(0).Skip(2));
    }
}