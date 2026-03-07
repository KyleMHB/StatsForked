using System;
using UnityEngine;

namespace Stats;

internal readonly record struct GUIScrollContext : IDisposable
{
    public GUIScrollContext(Rect rect, ref Vector2 scrollPosition, Rect viewRect, bool showScrollbars = true)
    {
        Verse.Widgets.BeginScrollView(rect, ref scrollPosition, viewRect, showScrollbars);
    }

    public void Dispose() => Verse.Widgets.EndScrollView();
}
