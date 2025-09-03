using Verse;

namespace Stats;

public sealed class ChairDefsTableWorker : ThingDefsTableWorker
{
    public ChairDefsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.building?.isSittable == true && thingDef.IsBuildingObtainableByPlayer();
    }
}
