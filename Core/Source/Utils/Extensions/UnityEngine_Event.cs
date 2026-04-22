using UnityEngine;

namespace Stats.Utils.Extensions;

public static class UnityEngine_Event
{
    internal static bool IsLeftMouseInteraction(this Event? @event)
    {
        return @event is
        {
            button: 0,
            type: EventType.MouseDown or EventType.MouseDrag or EventType.MouseUp
        };
    }
}
