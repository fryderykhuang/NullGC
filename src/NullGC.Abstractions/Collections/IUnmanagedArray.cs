namespace NullGC.Collections;

public interface IUnmanagedArray<T> : IHasUnmanagedResource
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// unmanaged constraint is not added to allow potential fixable managed pointer implementations. 
    /// </remarks>
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
    unsafe T* Items { get; }
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
    int Length { get; }
}