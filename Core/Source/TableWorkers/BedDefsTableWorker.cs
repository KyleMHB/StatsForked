using Verse;

namespace Stats;

public sealed class BedDefsTableWorker : ThingDefsTableWorker
{
    public BedDefsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.building != null
            && thingDef.IsBuildingObtainableByPlayer()
            && thingDef.IsBed;
    }
}
