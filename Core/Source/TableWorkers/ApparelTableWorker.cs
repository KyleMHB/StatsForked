using Verse;

namespace Stats;

public sealed class ApparelTableWorker : ThingTableWorker<Thing>
{
    public ApparelTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThing(Thing thing)
    {
        return thing.def.IsApparel;
    }
}
