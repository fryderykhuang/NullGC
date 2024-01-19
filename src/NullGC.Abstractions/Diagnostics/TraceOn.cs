using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NullGC.Diagnostics;

public static class TraceOn
{
    public static TextWriter TextWriter { get; set; }

    static TraceOn()
    {
        TextWriter = Console.Error;
    }

    private static void WriteHeader(string source, [CallerMemberName] string callerName = "")
    {
        TextWriter.Write(DateTime.UtcNow.ToString("T"));
        TextWriter.Write('|');
        TextWriter.Write(callerName);
        TextWriter.Write('|');
        TextWriter.Write(source);
        TextWriter.Write(':');
    }

    [Conditional("TRACE_MEM_ALLOC")]
    public static void MemAlloc(string source, string message)
    {
        WriteHeader(source);
        TextWriter.WriteLine(message);
    }

    [Conditional("TRACE_MEM_CACHE_LOST")]
    public static void MemCacheLost(string source, string message)
    {
        WriteHeader(source);
        TextWriter.WriteLine(message);
    }

    [Conditional("TRACE_MEM_RESIZE_MISS")]
    public static void MemResizeMiss(string source, string message)
    {
        WriteHeader(source);
        TextWriter.WriteLine(message);
    }
}