using Verse;

namespace Stats;

public sealed class ArtBuildingsTableWorker : ThingTableWorker
{
    public ArtBuildingsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsArt;
    }
}
