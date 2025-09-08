using UnityEngine;

namespace Stats.Widgets.Extensions;

public sealed class WidthAbsWidgetExtension : WidgetExtension
{
    private readonly float Width;
    internal WidthAbsWidgetExtension(Widget widget, float width) : base(widget)
    {
        Width = width;
    }
    public override Vector2 GetSize(Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with { x = Width };
    }
    public override Vector2 GetSize()
    {
        return Widget.GetSize() with { x = Width };
    }
}
