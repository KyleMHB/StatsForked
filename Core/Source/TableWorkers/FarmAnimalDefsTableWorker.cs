using RimWorld;
using Verse;

namespace Stats;

public sealed class FarmAnimalDefsTableWorker : ThingDefsTableWorker
{
    public FarmAnimalDefsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return
            thingDef is { race.Animal: true, IsCorpse: false }
            && (
                thingDef.GetCompProperties<CompProperties_Milkable>() != null
                || thingDef.GetCompProperties<CompProperties_EggLayer>() != null
                || thingDef.GetCompProperties<CompProperties_Shearable>() != null
            );
    }
}
