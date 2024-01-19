namespace NullGC.Linq;

/// <summary>
/// An interface exposes the possible count and maximum count for the items the enumerator may enumerate. 
/// </summary>
public interface IMaybeCountable
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Since <see cref="ISkipTakeAware.SetSkipCount"/> and <see cref="ISkipTakeAware.SetTakeCount"/> can affect the count, after calling this, no calls should be made to either <see cref="ISkipTakeAware.SetSkipCount"/> or <see cref="ISkipTakeAware.SetTakeCount"/> 
    /// </remarks>
    int? Count { get; }
    
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Since <see cref="ISkipTakeAware.SetSkipCount"/> and <see cref="ISkipTakeAware.SetTakeCount"/> can affect the count, after calling this, no calls should be made to either <see cref="ISkipTakeAware.SetSkipCount"/> or <see cref="ISkipTakeAware.SetTakeCount"/> 
    /// </remarks>
    int? MaxCount { get; }
}