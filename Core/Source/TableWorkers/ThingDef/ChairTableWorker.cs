using Stats;
using Verse;

namespace Stats.TableWorkers.ThingDef;

public sealed class ChairTableWorker : ThingDefTableWorker
{
    public ChairTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.building?.isSittable == true && thingDef.IsBuildingObtainableByPlayer();
    }
}
