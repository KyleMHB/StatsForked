using UnityEngine;

namespace Stats.Widgets.Extensions;

public sealed class WidthIncRelWidgetExtension : WidgetExtension
{
    private readonly float ParentWidthMultiplier;
    internal WidthIncRelWidgetExtension(
        Widget widget,
        float parentWidthMultiplier
    ) : base(widget)
    {
        ParentWidthMultiplier = parentWidthMultiplier;
    }
    protected override Vector2 CalcSize(Vector2 containerSize)
    {
        var size = Widget.GetSize(containerSize);

        return size with
        {
            x = size.x + containerSize.x * ParentWidthMultiplier
        };
    }
}
