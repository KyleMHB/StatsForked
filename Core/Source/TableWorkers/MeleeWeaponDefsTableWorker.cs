using Verse;

namespace Stats;

public sealed class MeleeWeaponDefsTableWorker : ThingDefsTableWorker
{
    public MeleeWeaponDefsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef is { IsMeleeWeapon: true, destroyOnDrop: false };
    }
}
