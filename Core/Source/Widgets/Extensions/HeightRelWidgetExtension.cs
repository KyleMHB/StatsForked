using UnityEngine;

namespace Stats.Widgets.Extensions;

public sealed class HeightRelWidgetExtension : WidgetExtension
{
    private readonly float ParentHeightMultiplier;
    internal HeightRelWidgetExtension(
        Widget widget,
        float parentHeightMultiplier
    ) : base(widget)
    {
        ParentHeightMultiplier = parentHeightMultiplier;
    }
    protected override Vector2 CalcSize(Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with
        {
            y = containerSize.y * ParentHeightMultiplier
        };
    }
}
