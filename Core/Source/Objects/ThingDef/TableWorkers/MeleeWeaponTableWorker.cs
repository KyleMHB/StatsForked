namespace Stats.Objects.ThingDef.TableWorkers;

public sealed class MeleeWeaponTableWorker : ThingDefTableWorker
{
    public MeleeWeaponTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(Verse.ThingDef thingDef)
    {
        return thingDef is { IsMeleeWeapon: true, destroyOnDrop: false };
    }
}
