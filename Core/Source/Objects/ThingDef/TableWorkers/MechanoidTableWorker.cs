namespace Stats.Objects.ThingDef.TableWorkers;

public sealed class MechanoidTableWorker : ThingDefTableWorker
{
    public MechanoidTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(Verse.ThingDef thingDef)
    {
        return thingDef is { race.IsMechanoid: true, IsCorpse: false };
    }
}
