using Verse;

namespace Stats;

public sealed class TurretDefTableWorker : ThingDefTableWorker
{
    public TurretDefTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.building?.IsTurret == true && thingDef.IsBuildingObtainableByPlayer();
    }
}
