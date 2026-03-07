using System;
using UnityEngine;

namespace Stats;

internal readonly record struct GUIClipContext : IDisposable
{
    public GUIClipContext(Rect rect)
    {
        GUI.BeginClip(rect);
    }

    public void Dispose() => GUI.EndClip();
}
