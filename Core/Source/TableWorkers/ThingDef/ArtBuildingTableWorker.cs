using Stats.Utils.Extensions;

namespace Stats.TableWorkers.ThingDef;

public class ArtBuildingTableWorker(TableDef tableDef) : ThingDefTableWorker(tableDef)
{
    protected override bool IsValidThingDef(Verse.ThingDef thingDef)
    {
        return thingDef.building != null
            && thingDef.IsBuildingObtainableByPlayer()
            && thingDef.IsArt;
    }
}
