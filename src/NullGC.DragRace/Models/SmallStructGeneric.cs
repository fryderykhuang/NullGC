namespace NullGC.DragRace.Models;

public struct SmallStructGeneric<TKey, TValue>
{
    public SmallStructGeneric(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }

    public TKey Key;
    public TValue Value;
}