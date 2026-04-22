using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Utils;
using Stats.Utils.Extensions;
using Verse;

namespace Stats;

internal static class InventoryStateTracker
{
    private static readonly HashSet<ThingDef> _ownedThingDefs = [];
    private static readonly HashSet<ThingDef> _mapVisibleThingDefs = [];
    private static int _lastRefreshTick = -1;
    private static bool _isDirty = true;

    static InventoryStateTracker()
    {
        Events.ThingSpawned += _ => _isDirty = true;
        Events.ThingDespawned += _ => _isDirty = true;
    }

    public static bool IsOwnedByPlayer(ThingDef thingDef)
    {
        RefreshIfNeeded();
        return _ownedThingDefs.Contains(thingDef);
    }

    public static bool IsVisibleOnPlayerMap(ThingDef thingDef)
    {
        RefreshIfNeeded();
        return _mapVisibleThingDefs.Contains(thingDef);
    }

    private static void RefreshIfNeeded()
    {
        int currentTick = Find.TickManager?.TicksGame ?? -1;
        if (_isDirty == false && currentTick == _lastRefreshTick)
        {
            return;
        }

        _isDirty = false;
        _lastRefreshTick = currentTick;
        _ownedThingDefs.Clear();
        _mapVisibleThingDefs.Clear();

        foreach (Map map in Find.Maps)
        {
            if (map.IsPlayerHome == false)
            {
                continue;
            }

            foreach (Thing thing in map.spawnedThings)
            {
                CountSpawnedThing(thing);
            }

            foreach (Pawn pawn in map.mapPawns.FreeColonistsSpawned)
            {
                CountPawnEquipment(pawn);
            }
        }
    }

    private static void CountSpawnedThing(Thing thing)
    {
        _ownedThingDefs.Add(thing.def);

        if (IsVisibleOnMap(thing))
        {
            _mapVisibleThingDefs.Add(thing.def);
        }
    }

    private static void CountPawnEquipment(Pawn pawn)
    {
        if (pawn.apparel?.WornApparel != null)
        {
            foreach (Apparel apparel in pawn.apparel.WornApparel)
            {
                _ownedThingDefs.Add(apparel.def);
            }
        }

        if (pawn.equipment?.AllEquipmentListForReading != null)
        {
            foreach (ThingWithComps equipment in pawn.equipment.AllEquipmentListForReading)
            {
                _ownedThingDefs.Add(equipment.def);
            }
        }

        if (pawn.inventory?.innerContainer != null)
        {
            foreach (Thing item in pawn.inventory.innerContainer)
            {
                _ownedThingDefs.Add(item.def);
            }
        }
    }

    private static bool IsVisibleOnMap(Thing thing)
    {
        return thing.Spawned
            && thing.MapHeld != null
            && thing.IsForbidden(Faction.OfPlayer) == false;
    }
}
