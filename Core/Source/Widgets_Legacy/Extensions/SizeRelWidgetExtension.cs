using UnityEngine;

namespace Stats.Widgets_Legacy.Extensions;

public sealed class SizeRelWidgetExtension : WidgetExtension
{
    private readonly float ParentWidthMultiplier;
    private readonly float ParentHeightMultiplier;
    internal SizeRelWidgetExtension(
        Widget widget,
        float parentWidthMultiplier,
        float parentHeightMultiplier
    ) : base(widget)
    {
        ParentWidthMultiplier = parentWidthMultiplier;
        ParentHeightMultiplier = parentHeightMultiplier;
    }
    public override Vector2 GetSize(Vector2 containerSize)
    {
        Vector2 size;
        size.x = ParentWidthMultiplier * containerSize.x;
        size.y = ParentHeightMultiplier * containerSize.y;

        return size;
    }
}
