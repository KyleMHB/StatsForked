using Verse;

namespace Stats;

public sealed class MeleeWeaponsTableWorker : AbstractThingTableWorker
{
    public MeleeWeaponsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef is { IsMeleeWeapon: true, destroyOnDrop: false };
    }
}
