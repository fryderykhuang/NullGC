using System.Reflection;
using Xunit.Abstractions;

namespace NullGC.TestCommons.Extensions;

public static class XUnitExtensions
{
    public static bool TryGetITest(this ITestOutputHelper o, out ITest test)
    {
        var type = o.GetType();
        var testMember = type.GetField("test", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty);
        test = (ITest) testMember?.GetValue(o)!;
        return test is not null;
    }
}