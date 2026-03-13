using Stats.Utils;
using UnityEngine;
using Verse;

namespace Stats.Widgets_Legacy;

public sealed class ThingIcon : Widget
{
    private readonly Thing Thing;

    public ThingIcon(Thing thing)
    {
        Thing = thing;
    }

    public override Vector2 GetSize()
    {
        return new Vector2(Text.LineHeight, Text.LineHeight);
    }

    public override void Draw(Rect rect, Vector2 _)
    {
        GUIDebugger.DebugRect(this, rect);

        if (Event.current.type == EventType.Repaint)
        {
            Verse.Widgets.ThingIcon(rect, Thing);
        }
    }
}
