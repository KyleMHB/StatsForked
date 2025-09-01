using RimWorld;
using Verse;

namespace Stats;

public sealed class RecreationalBuildingsTableWorker : AbstractThingTableWorker
{
    public RecreationalBuildingsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.building != null
            && thingDef.IsBuildingObtainableByPlayer()
            && thingDef.statBases?.GetStatValueFromList(StatDefOf.JoyGainFactor, 0f) > 0f;
    }
}
