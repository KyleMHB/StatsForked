using Stats.Extensions;

namespace Stats.TableWorkers.ThingDef;

public sealed class TurretTableWorker(TableDef tableDef) : ThingDefTableWorker(tableDef)
{
    protected override bool IsValidThingDef(Verse.ThingDef thingDef)
    {
        return thingDef.building?.IsTurret == true && thingDef.IsBuildingObtainableByPlayer();
    }
}
