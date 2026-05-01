using UnityEngine;

namespace Stats.Widgets_Legacy.Extensions;

public sealed class CalcWidthWidgetExtension : WidgetExtension
{
    private readonly SingleAxisSizeFunc WidthFunction;
    internal CalcWidthWidgetExtension(
        Widget widget,
        SingleAxisSizeFunc widthFunction
    ) : base(widget)
    {
        WidthFunction = widthFunction;
    }
    public override Vector2 GetSize(Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with
        { x = WidthFunction(containerSize) };
    }
}
