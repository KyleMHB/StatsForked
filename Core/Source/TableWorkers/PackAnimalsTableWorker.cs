using Verse;

namespace Stats;

public sealed class PackAnimalsTableWorker : AbstractThingTableWorker
{
    public PackAnimalsTableWorker(TableDef tableDef) : base(tableDef)
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
