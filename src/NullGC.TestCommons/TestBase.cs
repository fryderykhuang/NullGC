using NullGC.TestCommons.Extensions;
using Xunit.Abstractions;

namespace NullGC.TestCommons;

public abstract class TestBase : IDisposable
{
    private readonly ITestOutputHelper _logger;

    protected TestBase(ITestOutputHelper logger)
    {
        _logger = logger;
        if (logger.TryGetITest(out var test)) logger.WriteLine($"Starting test '{test.DisplayName}'");
        else throw new InvalidOperationException();
    }

    public virtual void Dispose()
    {
        if (_logger.TryGetITest(out var test)) _logger.WriteLine($"Finished test '{test.DisplayName}'");
        else throw new InvalidOperationException();
    }
}