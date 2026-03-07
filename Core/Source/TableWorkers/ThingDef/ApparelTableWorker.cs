namespace Stats.TableWorkers.ThingDef;

// We do not check for "destroyOnDrop" for better compatibility with mods like
// VFE - Pirates.
public sealed class ApparelTableWorker(TableDef tableDef) : ThingDefTableWorker(tableDef)
{
    protected override bool IsValidThingDef(Verse.ThingDef thingDef)
    {
        return thingDef.apparel != null;
    }
}
