namespace NullGC.Linq;

public interface ISkipTakeAware
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="count"></param>
    /// <returns>if true, the callee has accepted the skip count, caller enumerator must not skip.</returns>
    bool SetSkipCount(int count);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="count"></param>
    /// <returns>if true, the callee has accepted the take count, caller enumerator must not limit the take count.</returns>
    bool SetTakeCount(int count);
}