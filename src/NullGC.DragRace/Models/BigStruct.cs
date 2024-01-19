namespace NullGC.DragRace.Models;

public struct BigStruct
{
    public BigStruct(int key, float value)
    {
        Key = key;
        Value = value;
    }

    public int Key;
    public float Value;
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