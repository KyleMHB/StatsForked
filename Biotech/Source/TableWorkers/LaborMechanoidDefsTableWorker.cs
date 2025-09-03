using Verse;

namespace Stats.Compat.Biotech;

public sealed class LaborMechanoidDefsTableWorker : ThingDefsTableWorker
{
    public LaborMechanoidDefsTableWorker(TableDef tableDef) : base(tableDef)
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
