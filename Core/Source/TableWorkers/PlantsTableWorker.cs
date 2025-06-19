using Verse;

namespace Stats;

public sealed class PlantsTableWorker : ThingTableWorker
{
    public PlantsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsPlant && thingDef.plant?.isStump == false;
    }
}
