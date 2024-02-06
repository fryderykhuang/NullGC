using System.Globalization;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.EventProcessors;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Filters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.CoreRun;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
using BenchmarkDotNet.Validators;

namespace NullGC.DragRace;

internal class InProcConfig : ConfigBase
{
    public InProcConfig()
    {
        AddJob(Job.ShortRun//.WithArguments(new[] {new MsBuildArgument("/p:Platform=x64")})
                .WithToolchain(InProcessNoEmitToolchain.Instance))
            .WithOptions(ConfigOptions.DisableOptimizationsValidator)
            .AddDiagnoser(new MemoryDiagnoser(new MemoryDiagnoserConfig(true)))
            .AddLogger(new ConsoleLogger());
    }
}

internal class FastConfig : ConfigBase
{
    public FastConfig()
    {
        AddJob(Job.ShortRun)//.WithArguments(new[] {new MsBuildArgument("/p:Platform=x64")}))
            .WithOptions(ConfigOptions.DisableOptimizationsValidator)
            .AddDiagnoser(new MemoryDiagnoser(new MemoryDiagnoserConfig(true)))
            .AddLogger(new ConsoleLogger());
    }
}

internal class NormalConfig : ConfigBase
{
    public NormalConfig()
    {
        AddJob(Job.MediumRun)//.WithArguments(new[] {new MsBuildArgument("/p:Platform=x64")}))
            .AddDiagnoser(new MemoryDiagnoser(new MemoryDiagnoserConfig(true)))
            .AddLogger(new ConsoleLogger());
    }
}

internal class CicdConfig : ConfigBase
{
    public CicdConfig()
    {
        AddJob(Job.MediumRun)//.WithArguments(new[] {new MsBuildArgument("/p:Platform=x64")}))
            .AddDiagnoser(new MemoryDiagnoser(new MemoryDiagnoserConfig(true)))
            .AddExporter(new JsonExporter()).AddExporter(new HtmlExporter())
            .AddColumnProvider(DefaultColumnProviders.Instance)
            //.AddLogger(new ConsoleLogger())
            ;
    }
}

internal class ConfigBase : IConfig
{
    private static readonly Conclusion[] emptyConclusion = Array.Empty<Conclusion>();

    private readonly List<ILogger> loggers = new List<ILogger>();
    private readonly List<IExporter> exporters = new List<IExporter>();
    private readonly List<IDiagnoser> diagnosers = new List<IDiagnoser>();
    private readonly HashSet<HardwareCounter> hardwareCounters = new HashSet<HardwareCounter>();
    private readonly List<Job> jobs = new List<Job>();
    private string? _artifactsPath;

    protected ConfigBase()
    {
    }

    public ConfigBase AddLogger(params ILogger[] newLoggers)
    {
        this.loggers.AddRange(newLoggers);
        return this;
    }

    public ConfigBase AddJob(params Job[] newJobs)
    {
        this.jobs.AddRange(((IEnumerable<Job>) newJobs).Select<Job, Job>((Func<Job, Job>) (j => j.Freeze())));
        return this;
    }

    public ConfigBase AddDiagnoser(params IDiagnoser[] newDiagnosers)
    {
        this.diagnosers.AddRange((IEnumerable<IDiagnoser>) newDiagnosers);
        return this;
    }

    public ConfigBase AddExporter(params IExporter[] newExporters)
    {
        this.exporters.AddRange(newExporters);
        return this;
    }
    
    public ConfigBase AddHardwareCounters(params HardwareCounter[] newHardwareCounters)
    {
        foreach (var counter in newHardwareCounters)
        {
            this.hardwareCounters.Add(counter);
        }
        return this;
    }

    public IEnumerable<IColumnProvider> GetColumnProviders()
    {
        return DefaultColumnProviders.Instance;
    }

    public IEnumerable<IExporter> GetExporters()
    {
        return exporters;
    }

    public IEnumerable<ILogger> GetLoggers()
    {
        return loggers;
    }

    public IEnumerable<IAnalyser> GetAnalysers()
    {
        yield return EnvironmentAnalyser.Default;
        yield return OutliersAnalyser.Default;
        yield return MinIterationTimeAnalyser.Default;
        yield return MultimodalDistributionAnalyzer.Default;
        yield return RuntimeErrorAnalyser.Default;
        yield return ZeroMeasurementAnalyser.Default;
        yield return BaselineCustomAnalyzer.Default;
        yield return HideColumnsAnalyser.Default;
    }

    public IEnumerable<IValidator> GetValidators()
    {
        yield return (IValidator) BaselineValidator.FailOnError;
        yield return (IValidator) SetupCleanupValidator.FailOnError;
        yield return JitOptimizationsValidator.FailOnError;
        yield return RunModeValidator.FailOnError;
        yield return GenericBenchmarksValidator.DontFailOnError;
        yield return DeferredExecutionValidator.FailOnError;
        yield return (IValidator) ParamsAllValuesValidator.FailOnError;
        yield return (IValidator) ParamsValidator.FailOnError;
    }

    public IOrderer? Orderer => null;

    public ICategoryDiscoverer? CategoryDiscoverer => null;

    public ConfigUnionRule UnionRule => ConfigUnionRule.Union;

    public CultureInfo? CultureInfo => null;

    public ConfigOptions Options { get; set; } = ConfigOptions.Default;

    public SummaryStyle SummaryStyle => SummaryStyle.Default;

    public TimeSpan BuildTimeout => TimeSpan.FromSeconds(120.0);

    public string ArtifactsPath =>
        _artifactsPath
        ?? Environment.GetEnvironmentVariable("BenchmarkDotNet_ArtifactsPath")
        ?? Path.Combine(
            RuntimeInformation.RuntimeIdentifier.Contains("Android", StringComparison.InvariantCultureIgnoreCase)
                ? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
                : Directory.GetCurrentDirectory(), "BenchmarkDotNet.Artifacts");

    public IReadOnlyList<Conclusion> ConfigAnalysisConclusion => emptyConclusion;

    public IEnumerable<Job> GetJobs() => this.jobs;

    public IEnumerable<BenchmarkLogicalGroupRule> GetLogicalGroupRules()
    {
        return Array.Empty<BenchmarkLogicalGroupRule>();
    }

    public IEnumerable<IDiagnoser> GetDiagnosers()
    {
        return diagnosers;
    }

    public IEnumerable<HardwareCounter> GetHardwareCounters()
    {
        return hardwareCounters;
    }

    public IEnumerable<IFilter> GetFilters() => Array.Empty<IFilter>();

    public IEnumerable<EventProcessor> GetEventProcessors()
    {
        return Array.Empty<EventProcessor>();
    }

    public IEnumerable<IColumnHidingRule> GetColumnHidingRules()
    {
        return Array.Empty<IColumnHidingRule>();
    }

    public ConfigBase WithOptions(ConfigOptions options)
    {
        this.Options |= options;
        return this;
    }

    public ConfigBase WithArtifactsPath(string artifactsPath)
    {
        this._artifactsPath = artifactsPath;
        return this;
    }
}