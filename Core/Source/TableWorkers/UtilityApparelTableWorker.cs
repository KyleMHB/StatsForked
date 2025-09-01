using Verse;

namespace Stats;

public sealed class UtilityApparelTableWorker : AbstractThingTableWorker
{
    private readonly ThingCategoryDef UtilityCatDef =
        DefDatabase<ThingCategoryDef>.GetNamed("ApparelUtility");
    public UtilityApparelTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsApparel && thingDef.IsWithinCategory(UtilityCatDef);
    }
}
