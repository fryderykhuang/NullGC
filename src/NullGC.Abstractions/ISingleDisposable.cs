namespace NullGC;

/// <summary>
/// Should be implemented on a struct typed <typeparamref name="T"/> that does not support repeated Dispose() call.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// When the owner of <typeparamref name="T"/> wants to pass it as value or return as value, call the <see cref="Borrow"/> method.
/// </remarks>
public interface ISingleDisposable<out T> : IDisposable where T : struct, IDisposable
{
    /// <summary>
    /// Get a copy of the implementer itself that will not cause double-dispose problem (e.g. by set a non-dispose flag).
    /// </summary>
    /// <returns></returns>
    T Borrow();
}
