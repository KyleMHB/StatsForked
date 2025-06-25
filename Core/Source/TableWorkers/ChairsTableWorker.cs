using Verse;

namespace Stats;

public sealed class ChairsTableWorker : ThingTableWorker
{
    public ChairsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.building?.isSittable == true && thingDef.IsBuildingObtainableByPlayer();
    }
}
