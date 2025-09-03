using Verse;

namespace Stats;

public sealed class UtilityApparelDefsTableWorker : ThingDefsTableWorker
{
    private readonly ThingCategoryDef UtilityCatDef =
        DefDatabase<ThingCategoryDef>.GetNamed("ApparelUtility");
    public UtilityApparelDefsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsApparel && thingDef.IsWithinCategory(UtilityCatDef);
    }
}
