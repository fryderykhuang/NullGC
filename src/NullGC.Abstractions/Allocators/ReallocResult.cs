namespace NullGC.Allocators;

public readonly struct ReallocResult
{
    public readonly UIntPtr Ptr;
    public readonly nuint ActualSize;

    public static readonly ReallocResult NotSuccess = new ReallocResult();  

    public bool Success => Ptr != UIntPtr.Zero;

    public ReallocResult(UIntPtr ptr, UIntPtr actualSize)
    {
        Ptr = ptr;
        ActualSize = actualSize;
    }
}