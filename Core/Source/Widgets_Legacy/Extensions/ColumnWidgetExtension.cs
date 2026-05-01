using System.Runtime.CompilerServices;
using UnityEngine;

namespace Stats.Widgets_Legacy.Extensions;

internal sealed class ColumnWidgetExtension : WidgetExtension
{
    private readonly StrongBox<float> Width;
    internal ColumnWidgetExtension(Widget widget, StrongBox<float> width) : base(widget)
    {
        Width = width;
    }
    public override Vector2 GetSize(Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with { x = Width.Value };
    }
    public override Vector2 GetSize()
    {
        return Widget.GetSize() with { x = Width.Value };
    }
}
