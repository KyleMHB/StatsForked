using Verse;

namespace Stats;

public sealed class PackAnimalDefsTableWorker : ThingDefsTableWorker
{
    public PackAnimalDefsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef is
        {
            race:
            {
                Animal: true,
                packAnimal: true
            },
            IsCorpse: false
        };
    }
}
