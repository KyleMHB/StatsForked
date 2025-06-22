using Verse;

namespace Stats.Compat.Biotech;

public sealed class LaborMechanoidsTableWorker : ThingTableWorker
{
    public LaborMechanoidsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef is
        {
            race:
            {
                IsMechanoid: true,
                mechEnabledWorkTypes.Count: > 0
            },
            IsCorpse: false
        };
    }
}
