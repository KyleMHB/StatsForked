namespace Stats.TableWorkers.ThingDef;

public sealed class AnimalTableWorker(TableDef tableDef) : ThingDefTableWorker(tableDef)
{
    protected override bool IsValidThingDef(Verse.ThingDef thingDef)
    {
        return thingDef is { race.Animal: true, IsCorpse: false };
    }
}
