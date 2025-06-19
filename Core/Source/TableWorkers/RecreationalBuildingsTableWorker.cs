using RimWorld;
using Verse;

namespace Stats;

public sealed class RecreationalBuildingsTableWorker : ThingTableWorker
{
    public RecreationalBuildingsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.statBases?.GetStatValueFromList(StatDefOf.JoyGainFactor, 0f) > 0f;
    }
}
