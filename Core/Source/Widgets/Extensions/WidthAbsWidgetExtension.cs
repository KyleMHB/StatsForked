using UnityEngine;

namespace Stats.Widgets.Extensions;

public sealed class WidthAbsWidgetExtension : WidgetExtension
{
    private readonly float Width;
    internal WidthAbsWidgetExtension(Widget widget, float width) : base(widget)
    {
        Width = width;
    }
    protected override Vector2 CalcSize(Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with { x = Width };
    }
    protected override Vector2 CalcSize()
    {
        return Widget.GetSize() with { x = Width };
    }
}
