using Verse;

namespace Stats;

public sealed class MechanoidsTableWorker : AbstractThingTableWorker
{
    public MechanoidsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef is { race.IsMechanoid: true, IsCorpse: false };
    }
}
