using UnityEngine;

namespace Stats.Widgets;

public sealed class EmptyWidget : Widget
{
    public EmptyWidget()
    {
    }
    public override Vector2 GetSize()
    {
        return Vector2.zero;
    }
    public override void Draw(Rect rect, Vector2 _)
    {
        GUIDebugger.DebugRect(this, rect);
    }
}
