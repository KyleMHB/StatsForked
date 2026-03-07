using RimWorld;

namespace Stats.TableWorkers.ThingDef;

public sealed class RangedWeaponTableWorker(TableDef tableDef) : ThingDefTableWorker(tableDef)
{
    protected override bool IsValidThingDef(Verse.ThingDef thingDef)
    {
        return thingDef is { IsRangedWeapon: true, destroyOnDrop: false }
            && thingDef.GetCompProperties<CompProperties_UniqueWeapon>() == null;
    }
}
