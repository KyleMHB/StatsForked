using System.Runtime.CompilerServices;
using UnityEngine;

namespace Stats.Utils.Extensions;

internal static class UnityEngine_Event
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsRepaint(this Event @event)
    {
        return @event.type == EventType.Repaint;
    }

    public static bool IsLMB(this Event @event)
    {
        return @event.button == 0;
    }

    public static bool IsRMB(this Event @event)
    {
        return @event.button == 1;
    }
}
