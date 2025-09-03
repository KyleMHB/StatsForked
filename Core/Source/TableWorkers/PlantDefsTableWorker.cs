using Verse;

namespace Stats;

public sealed class PlantDefsTableWorker : ThingDefsTableWorker
{
    public PlantDefsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsPlant && thingDef.plant?.isStump == false;
    }
}
