using Verse;

namespace Stats;

public sealed class MechanoidDefsTableWorker : ThingDefsTableWorker
{
    public MechanoidDefsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef is { race.IsMechanoid: true, IsCorpse: false };
    }
}
