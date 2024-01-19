namespace NullGC.Linq.Tests;

public struct BigStruct<TKey, TValue>
{
    public BigStruct(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }

    public TKey Key;
    public TValue Value;
    public long A1;
    public long A2;
    public long A3;
    public long A4;
    public long A5;
    public long A6;
    public long A7;
    public long A8;
    public long A9;
    public long A10;
}

public struct SmallStruct<TKey, TValue>
{
    public SmallStruct(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }

    public TKey Key;
    public TValue Value;
}