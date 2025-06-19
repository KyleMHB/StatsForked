using Verse;

namespace Stats;

public sealed class BedsTableWorker : ThingTableWorker
{
    public BedsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsBed;
    }
}
