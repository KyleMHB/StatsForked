using Verse;

namespace Stats;

public sealed class PawnsTableWorker : SpawnedThingTableWorker
{
    public PawnsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThing(Thing thing)
    {
        return thing.def.race?.Humanlike == true;
    }
}
