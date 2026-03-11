using System;
using UnityEngine;

namespace Stats.GUIScopes;

internal readonly record struct GUIClipScope : IDisposable
{
    public GUIClipScope(Rect rect)
    {
        GUI.BeginClip(rect);
    }

    public void Dispose() => GUI.EndClip();
}
