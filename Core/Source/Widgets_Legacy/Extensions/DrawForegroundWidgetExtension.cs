using System;
using UnityEngine;

namespace Stats.Widgets_Legacy.Extensions;

internal sealed class DrawForegroundWidgetExtension : WidgetExtension
{
    private readonly Action<Rect> DrawForeground;
    internal DrawForegroundWidgetExtension(
        Widget widget,
        Action<Rect> drawForeground
    ) : base(widget)
    {
        DrawForeground = drawForeground;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        Widget.Draw(rect, containerSize);

        if (Event.current.type == EventType.Repaint)
        {
            DrawForeground(rect);
        }
    }
}
