namespace Stats.Objects.ThingDef.TableWorkers;

public sealed class TurretTableWorker : ThingDefTableWorker
{
    public TurretTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(Verse.ThingDef thingDef)
    {
        return thingDef.building?.IsTurret == true && thingDef.IsBuildingObtainableByPlayer();
    }
}
