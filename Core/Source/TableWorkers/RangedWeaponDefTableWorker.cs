using RimWorld;
using Verse;

namespace Stats;

public sealed class RangedWeaponDefTableWorker : ThingDefTableWorker
{
    public RangedWeaponDefTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef is { IsRangedWeapon: true, destroyOnDrop: false }
            && thingDef.GetCompProperties<CompProperties_UniqueWeapon>() == null;
    }
}
