using UnityEngine;

namespace Stats.Widgets.Extensions;

internal class ToggleDisplayWidgetExtension : WidgetExtension
{
    private readonly Observable<bool> State;
    internal ToggleDisplayWidgetExtension(Widget widget, Observable<bool> state) : base(widget)
    {
        State = state;
        state.OnNext += value => Resize();
    }
    protected override Vector2 CalcSize(Vector2 containerSize)
    {
        if (State.Value)
        {
            return Widget.GetSize(containerSize);
        }

        return Vector2.zero;
    }
    protected override Vector2 CalcSize()
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
