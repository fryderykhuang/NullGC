namespace NullGC.Collections;

public interface IUnsafeArray<T>
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// unmanaged constraint is not added to facilitate <
    /// </remarks>
    unsafe T* Items { get; }
    int Length { get; }
    bool IsInitialized { get; }
}