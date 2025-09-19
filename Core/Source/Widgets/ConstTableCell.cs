using System;
using UnityEngine;

namespace Stats.Widgets;

public abstract class AbstractConstTableCell<T> : ObjectTable.Cell where T : IComparable<T>
{
    private readonly T Value;
    public AbstractConstTableCell(T value)
    {
        Value = value;
    }
    public override int CompareTo(ObjectTable.Cell cell)
    {
        return Value.CompareTo(GetValue(cell));
    }
    public static T GetValue(ObjectTable.Cell cell)
    {
        return ((AbstractConstTableCell<T>)cell).Value;
    }
}

public sealed class ConstTableCell<T> : AbstractConstTableCell<T> where T : IComparable<T>
{
    private readonly Widget Widget;
    public ConstTableCell(T value, Widget widget) : base(value)
    {
        Widget = widget;
    }
    public override Vector2 GetSize()
    {
        return Widget.GetSize();
    }
    public override Vector2 GetSize(Vector2 containerSize)
    {
        return Widget.GetSize(containerSize);
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        Widget.Draw(rect, containerSize);
    }
}

public sealed class EmptyConstTableCell<T> : AbstractConstTableCell<T> where T : IComparable<T>
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
