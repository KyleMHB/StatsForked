using Verse;

namespace Stats;

public sealed class AnimalsTableWorker : AbstractThingTableWorker
{
    public AnimalsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef is { race.Animal: true, IsCorpse: false };
    }
}
