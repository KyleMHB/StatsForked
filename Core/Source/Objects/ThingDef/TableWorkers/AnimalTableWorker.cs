namespace Stats.Objects.ThingDef.TableWorkers;

public sealed class AnimalTableWorker : ThingDefTableWorker
{
    public AnimalTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(Verse.ThingDef thingDef)
    {
        return thingDef is { race.Animal: true, IsCorpse: false };
    }
}
