using UnityEngine;

namespace Stats.Widgets.Extensions;

public sealed class WidthRelWidgetExtension : WidgetExtension
{
    private readonly float ParentWidthMultiplier;
    internal WidthRelWidgetExtension(
        Widget widget,
        float parentWidthMultiplier
    ) : base(widget)
    {
        ParentWidthMultiplier = parentWidthMultiplier;
    }
    public override Vector2 GetSize(Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with
        {
            x = containerSize.x * ParentWidthMultiplier
        };
    }
}
