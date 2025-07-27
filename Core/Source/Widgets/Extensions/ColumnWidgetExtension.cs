using System.Runtime.CompilerServices;
using UnityEngine;

namespace Stats.Widgets.Extensions;

internal sealed class ColumnWidgetExtension : WidgetExtension
{
    private readonly StrongBox<float> Width;
    internal ColumnWidgetExtension(Widget widget, StrongBox<float> width) : base(widget)
    {
        Width = width;
    }
    protected override Vector2 CalcSize(Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with { x = Width.Value };
    }
    protected override Vector2 CalcSize()
    {
        return Widget.GetSize() with { x = Width.Value };
    }
}
