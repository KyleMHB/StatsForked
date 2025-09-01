using Verse;

namespace Stats;

public sealed class PlantsTableWorker : AbstractThingTableWorker
{
    public PlantsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsPlant && thingDef.plant?.isStump == false;
    }
}
