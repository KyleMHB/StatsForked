using Verse;

namespace Stats;

public sealed class TurretsTableWorker : ThingTableWorker
{
    public TurretsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.building?.IsTurret == true && thingDef.IsBuildingObtainableByPlayer();
    }
}
