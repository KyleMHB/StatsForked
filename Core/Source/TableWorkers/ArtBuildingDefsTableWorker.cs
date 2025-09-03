using Verse;

namespace Stats;

public sealed class ArtBuildingDefsTableWorker : ThingDefsTableWorker
{
    public ArtBuildingDefsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.building != null
            && thingDef.IsBuildingObtainableByPlayer()
            && thingDef.IsArt;
    }
}
