using Verse;

namespace Stats;

public sealed class ApparelTableWorker : ThingTableWorker
{
    private readonly ThingCategoryDef UtilityCatDef = DefDatabase<ThingCategoryDef>.GetNamed("ApparelUtility");
    public ApparelTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsApparel && thingDef.IsWithinCategory(UtilityCatDef) == false;
    }
}
