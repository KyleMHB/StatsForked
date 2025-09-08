using Verse;

namespace Stats;

public sealed class PlantDefTableWorker : ThingDefTableWorker
{
    public PlantDefTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsPlant && thingDef.plant?.isStump == false;
    }
}
