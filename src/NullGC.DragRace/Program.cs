// See https://aka.ms/new-console-template for more information

// #define CICD
using System.Reflection;
using BenchmarkDotNet.Running;
using NullGC.DragRace;
using NullGC.DragRace.Benchmarks;
// TODO
#if CICD
// var cfg = new CicdConfig();
// Console.WriteLine($"ArtifactsPath={cfg.ArtifactsPath}");
// BenchmarkRunner.Run<IntListGrowingBenchmarks>(cfg);
// return;
Console.Error.WriteLine("Running on CICD profile.");
BenchmarkRunner.Run(
    Assembly.GetExecutingAssembly().GetExportedTypes()
        .Where(t => t.CustomAttributes.Any(a => a.AttributeType == typeof(PickedForCicd))).ToArray(), new CicdConfig());
return;
#else

var types = args.Select(Type.GetType).Where(x => x is not null).ToArray();
if (types.Length > 0)
{
    BenchmarkRunner.Run(types!, new FastConfig());
    return;
}

BenchmarkRunner.Run<IntListGrowingBenchmarks>(new FastConfig());
return;

IntArrayAllocOverTime();

void IntArrayAllocOverTime()
{
    var bench = new IntArrayAllocationOverTimeBenchmarks();
    bench.Setup();

    for (var i = 0; i < 1000; i++)
    {
        bench.IterationSetup();
        bench.NewValueIntNoClear();
        // bench.IterationCleanup();
    }

    Console.WriteLine("Paused.");
    Console.ReadLine();

    for (var i = 0; i < 2000; i++)
    {
        bench.IterationSetup();
        bench.NewValueIntNoClear();
        // bench.IterationCleanup();
    }

    Console.WriteLine("Done.");
    Console.ReadLine();
}


void IntListGrow()
{
    var bench = new IntListGrowingBenchmarks();
    bench.Setup();

    for (var i = 0; i < 1; i++)
    {
        bench.IterationSetup();
        bench.ValueList();
        bench.IterationCleanup();
    }

    Console.WriteLine("Paused.");
    Console.ReadLine();

    for (var i = 0; i < 1; i++)
    {
        bench.IterationSetup();
        bench.ValueList();
        bench.IterationCleanup();
    }

    Console.WriteLine("Done.");
    Console.ReadLine();
}

void CacheVsNativeAllocator()
{
    var bench = new CachedVsNativeAllocatorBenchmarks();
    bench.Setup();

    for (var i = 0; i < 2; i++) bench.Cached();

    Console.WriteLine("Paused.");
    Console.ReadLine();

    for (var i = 0; i < 200; i++) bench.Cached();

    Console.WriteLine("Done.");
    Console.ReadLine();
}


void NewIntArrayAllocator()
{
    var bench = new IntArrayAllocationBenchmarks();
    bench.Setup();

    bench.NewValueInt();

    for (var i = 0; i < 100; i++) bench.NewValueInt();

    Console.WriteLine("Paused.");
    Console.ReadLine();

    for (var i = 0; i < 1000; i++) bench.NewValueInt();

    Console.WriteLine("Done.");
    Console.ReadLine();
}

void SmallStruct()
{
    var bench = new SmallStruct_WhereSelectOrderByTakeMinBenchmarks();
    bench.Setup();

    bench.NullGCLinqRef_SmallStruct();

    for (var i = 0; i < 1000; i++) bench.NullGCLinqRef_SmallStruct();

    Console.WriteLine("Paused.");
    Console.ReadLine();

    for (var i = 0; i < 5000; i++) bench.NullGCLinqRef_SmallStruct();

    Console.WriteLine("Done.");
    Console.ReadLine();
}

void BigStruct()
{
    var bench = new BigStruct_WhereSelectOrderByTakeMin_MultiQueries_Benchmarks();
    bench.Setup();

    bench.NullGCLinqRef_BigStruct();

    for (var i = 0; i < 1000; i++) bench.NullGCLinqRef_BigStruct();

    Console.WriteLine("Paused.");
    Console.ReadLine();

    for (var i = 0; i < 5000; i++) bench.NullGCLinqRef_BigStruct();

    Console.WriteLine("Done.");
    Console.ReadLine();
}
// Allocator();
// BigStruct();


// Thread.Sleep(2000);
//
// for (int i = 0; i < 5000; i++)
// {
//     bench.BigStructValArrNullGCLinqRefWhereOrderByTakeAveragePtr();
// }
//
// Thread.Sleep(1000);
//
// for (int i = 0; i < 5000; i++)
// {
//     bench.BigStructArrSystemLinqWhereOrderByTakeAverage();
// }
// bench.Cleanup();


#endif