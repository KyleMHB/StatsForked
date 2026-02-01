using RimWorld;
using Stats;
using Verse;

namespace Stats.Objects.ThingDef.TableWorkers;

public sealed class RecreationalBuildingTableWorker : ThingDefTableWorker
{
    public RecreationalBuildingTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.building != null
            && thingDef.IsBuildingObtainableByPlayer()
            && thingDef.statBases?.GetStatValueFromList(StatDefOf.JoyGainFactor, 0f) > 0f;
    }
}
