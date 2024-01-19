namespace NullGC.DragRace.Models;

public struct SmallStruct
{
    public SmallStruct(int key, float value)
    {
        Key = key;
        Value = value;
    }

    public int Key;
    public float Value;
}