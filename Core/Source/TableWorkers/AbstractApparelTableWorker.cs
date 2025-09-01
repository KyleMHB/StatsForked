using Verse;

namespace Stats;

public sealed class AbstractApparelTableWorker : AbstractThingTableWorker
{
    private readonly ThingCategoryDef UtilityCatDef = DefDatabase<ThingCategoryDef>.GetNamed("ApparelUtility");
    public AbstractApparelTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        // We do not check for "destroyOnDrop" for better compatibility with mods like
        // VFE - Pirates.
        return thingDef.IsApparel && thingDef.IsWithinCategory(UtilityCatDef) == false;
    }
}
