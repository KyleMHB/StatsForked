namespace Stats.TableWorkers.ThingDef;

public sealed class PlantTableWorker : ThingDefTableWorker
{
    public PlantTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(Verse.ThingDef thingDef)
    {
        return thingDef.IsPlant && thingDef.plant?.isStump == false;
    }
}
