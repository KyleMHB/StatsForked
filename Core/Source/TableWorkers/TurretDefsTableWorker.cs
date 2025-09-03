using Verse;

namespace Stats;

public sealed class TurretDefsTableWorker : ThingDefsTableWorker
{
    public TurretDefsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.building?.IsTurret == true && thingDef.IsBuildingObtainableByPlayer();
    }
}
