namespace Stats.TableWorkers.ThingDef;

public class MeleeWeaponTableWorker(TableDef tableDef) : ThingDefTableWorker(tableDef)
{
    protected override bool IsValidThingDef(Verse.ThingDef thingDef)
    {
        return thingDef is { IsMeleeWeapon: true, destroyOnDrop: false };
    }
}
