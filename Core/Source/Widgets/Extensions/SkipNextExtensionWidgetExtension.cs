using System;
using UnityEngine;

namespace Stats.Widgets.Extensions;

internal sealed class SkipNextExtensionWidgetExtension : WidgetExtension
{
    private readonly Func<bool> Predicate;
    internal SkipNextExtensionWidgetExtension(WidgetExtension extension, Func<bool> predicate) : base(extension)
    {
        Predicate = predicate;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        if (Predicate())
        {
            ((WidgetExtension)Widget).Widget.Draw(rect, containerSize);
        }
        else
        {
            Widget.Draw(rect, containerSize);
        }
    }
}
