using Verse;

namespace Stats;

public sealed class ChairsTableWorker : AbstractThingTableWorker
{
    public ChairsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.building?.isSittable == true && thingDef.IsBuildingObtainableByPlayer();
    }
}
