using System.Runtime.CompilerServices;

namespace NullGC;

/// <summary>
/// Should be implemented on a struct typed <typeparamref name="T"/> that does not support repeated Dispose() call.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// When the owner of <typeparamref name="T"/> wants to pass it as value or return as value, call the <see cref="Borrow"/> method.
/// </remarks>
public interface IExplicitOwnership<out T> : IDisposable where T : struct, IDisposable
{
    /// <summary>
    /// Get a copy of the implementer itself that will not cause double-free problem (e.g. by set a non-dispose flag).
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// Since this is not a built-in lifetime/ownership management system, the actual lifetime is not being enforced, the early dispose from the owner side or mutation from the borrower side is still unpreventable thus should be used with caution.
    /// </remarks>
    [return: Borrowed]
    T Borrow();

    /// <summary>
    /// Get a copy of the implementer itself which will become the new owner and `this` will become a borrowed one.
    /// </summary>
    /// <returns></returns>
    [return: Owned]
    T Take();
}