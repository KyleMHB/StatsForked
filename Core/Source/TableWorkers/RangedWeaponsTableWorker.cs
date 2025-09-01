using RimWorld;
using Verse;

namespace Stats;

public sealed class RangedWeaponsTableWorker : AbstractThingTableWorker
{
    public RangedWeaponsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef is { IsRangedWeapon: true, destroyOnDrop: false }
            && thingDef.GetCompProperties<CompProperties_UniqueWeapon>() == null;
    }
}
