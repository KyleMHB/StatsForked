using Stats;
using Verse;

namespace Stats.Objects.ThingDef.TableWorkers;

public sealed class BedTableWorker : ThingDefTableWorker
{
    public BedTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.building != null
            && thingDef.IsBuildingObtainableByPlayer()
            && thingDef.IsBed;
    }
}
