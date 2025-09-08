using UnityEngine;

namespace Stats.Widgets.Extensions;

public sealed class SizeAbsWidgetExtension : WidgetExtension
{
    private readonly Vector2 Size;
    internal SizeAbsWidgetExtension(Widget widget, float width, float height) : base(widget)
    {
        Size.x = width;
        Size.y = height;
    }
    public override Vector2 GetSize(Vector2 _)
    {
        return Size;
    }
    public override Vector2 GetSize()
    {
        return Size;
    }
}
