using System.Runtime.CompilerServices;

namespace NullGC.Extensions;

public static class DisposeExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryDispose(this object? obj)
    {
        if (obj is IDisposable disp)
        {
            disp.Dispose();
            return true;
        }

        return false;
    }
}