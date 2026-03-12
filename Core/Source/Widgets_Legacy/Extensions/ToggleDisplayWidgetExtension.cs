using UnityEngine;

namespace Stats.Widgets_Legacy.Extensions;

internal class ToggleDisplayWidgetExtension : WidgetExtension
{
    private readonly Observable<bool> State;
    internal ToggleDisplayWidgetExtension(Widget widget, Observable<bool> state) : base(widget)
    {
        State = state;
        state.OnNext += value => Resize();
    }
    public override Vector2 GetSize(Vector2 containerSize)
    {
        if (State.Value)
        {
            return Widget.GetSize(containerSize);
        }

        return Vector2.zero;
    }
    public override Vector2 GetSize()
    {
        if (State.Value)
        {
            return Widget.GetSize();
        }

        return Vector2.zero;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        if (State.Value)
        {
            Widget.Draw(rect, containerSize);
        }
    }
}
