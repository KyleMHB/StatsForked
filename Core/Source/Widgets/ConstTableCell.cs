using System;
using UnityEngine;

namespace Stats.Widgets;

public abstract class ConstTableCell<T> : ObjectTable.Cell where T : IComparable<T>
{
    private readonly T Value;
    //private readonly Widget? Widget;
    public ConstTableCell(T value)
    {
        Value = value;
        //Widget = widget;
    }
    public override int CompareTo(ObjectTable.Cell cell)
    {
        return Value.CompareTo(GetValue(cell));
    }
    public static T GetValue(ObjectTable.Cell cell)
    {
        return ((ConstTableCell<T>)cell).Value;
    }
}

public sealed class EmptyConstTableCell<T> : ConstTableCell<T> where T : IComparable<T>
{
    public EmptyConstTableCell(T value) : base(value)
    {
    }
    public override Vector2 GetSize()
    {
        return Vector2.zero;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
    }
}
