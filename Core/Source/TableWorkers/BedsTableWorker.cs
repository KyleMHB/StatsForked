using Verse;

namespace Stats;

public sealed class BedsTableWorker : AbstractThingTableWorker
{
    public BedsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.building != null
            && thingDef.IsBuildingObtainableByPlayer()
            && thingDef.IsBed;
    }
}
