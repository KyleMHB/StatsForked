namespace Stats.TableWorkers.ThingDef;

public sealed class MeleeWeaponTableWorker(TableDef tableDef) : ThingDefTableWorker(tableDef)
{
    protected override bool IsValidThingDef(Verse.ThingDef thingDef)
    {
        return thingDef is { IsMeleeWeapon: true, destroyOnDrop: false };
    }
}
