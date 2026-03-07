using RimWorld;

namespace Stats.TableWorkers.ThingDef;

public sealed class PowerGeneratorTableWorker(TableDef tableDef) : ThingDefTableWorker(tableDef)
{
    protected override bool IsValidThingDef(Verse.ThingDef thingDef)
    {
        // Why not just "return thingDef.HasComp<CompPowerPlant>();"?
        //
        // For better compatibility.
        // For example, nuclear reactor in VFE-Power uses custom comp class,
        // that derives from CompPowerTrader and not from CompPowerPlant.
        return thingDef.building != null
            && thingDef.IsBuildingObtainableByPlayer()
            && thingDef.GetCompProperties<CompProperties_Power>()?.PowerConsumption < 0f;
    }
}
