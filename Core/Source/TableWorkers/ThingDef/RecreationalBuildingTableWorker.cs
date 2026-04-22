using RimWorld;
using Stats.Utils.Extensions;

namespace Stats.TableWorkers.ThingDef;

public class RecreationalBuildingTableWorker(TableDef tableDef) : ThingDefTableWorker(tableDef)
{
    protected override bool IsValidThingDef(Verse.ThingDef thingDef)
    {
        return thingDef.building != null
            && thingDef.IsBuildingObtainableByPlayer()
            && thingDef.statBases?.GetStatValueFromList(StatDefOf.JoyGainFactor, 0f) > 0f;
    }
}
