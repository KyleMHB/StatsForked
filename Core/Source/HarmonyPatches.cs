using HarmonyLib;
using Verse;

namespace Stats;

[StaticConstructorOnStartup]
public static class HarmonyPatches
{
    static HarmonyPatches()
    {
        var harmony = new Harmony("Azzkiy.Stats");

        harmony.Patch(
            AccessTools.Method(typeof(MapEvents), nameof(MapEvents.Notify_ThingSpawned)),
            postfix: new HarmonyMethod(typeof(Globals.Events), nameof(Globals.Events.NotifyThingSpawned))
        );
        harmony.Patch(
            AccessTools.Method(typeof(MapEvents), nameof(MapEvents.Notify_ThingDespawned)),
            postfix: new HarmonyMethod(typeof(Globals.Events), nameof(Globals.Events.NotifyThingDespawned))
        );
    }
}
