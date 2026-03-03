using RimWorld;

namespace Stats.TableWorkers.ThingDef;

public sealed class RangedWeaponTableWorker : ThingDefTableWorker
{
    public RangedWeaponTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(Verse.ThingDef thingDef)
    {
        return thingDef is { IsRangedWeapon: true, destroyOnDrop: false }
            && thingDef.GetCompProperties<CompProperties_UniqueWeapon>() == null;
    }
}
