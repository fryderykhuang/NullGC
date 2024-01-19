namespace NullGC;

public static class MemoryMath
{
    public static ulong Ceiling(ulong addr, uint align)
    {
        return (addr + ((ulong) align - 1)) & ~((ulong) align - 1);
    }

    public static uint Ceiling(uint addr, uint align)
    {
        return (addr + (align - 1)) & ~(align - 1);
    }

    public static int Floor(int addr, int align)
    {
        return addr & ~(align - 1);
    }

    public static int Ceiling(int addr, int align)
    {
        return (addr + (align - 1)) & ~(align - 1);
    }

    public static nint Ceiling(nint addr, int align)
    {
        return (addr + (align - 1)) & ~((nint) align - 1);
    }

    public static nuint Ceiling(nuint addr, uint align)
    {
        return (addr + (align - 1)) & ~((nuint) align - 1);
    }
    public static nuint Ceiling(nuint addr, nuint align)
    {
        return (addr + (align - 1)) & ~(align - 1);
    }
}