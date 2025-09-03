using Verse;

namespace Stats;

public sealed class PowerGeneratorDefsTableWorker : ThingDefsTableWorker
{
    public PowerGeneratorDefsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        // Why not just "return thingDef.HasComp<CompPowerPlant>();"?
        //
        // For better compatibility.
        // For example, nuclear reactor in VFE-Power uses custom comp class,
        // that derives from CompPowerTrader and not from CompPowerPlant.
        return thingDef.building != null
            && thingDef.IsBuildingObtainableByPlayer()
            && thingDef.GetPowerCompProperties()?.PowerConsumption < 0f;
    }
}
