using Verse;

namespace Stats;

public sealed class MeleeWeaponDefTableWorker : ThingDefTableWorker
{
    public MeleeWeaponDefTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef is { IsMeleeWeapon: true, destroyOnDrop: false };
    }
}
