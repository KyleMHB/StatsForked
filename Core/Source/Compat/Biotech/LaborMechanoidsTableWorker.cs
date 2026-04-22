using Stats.TableWorkers.ThingDef;

namespace Stats.Compat.Biotech;

public sealed class LaborMechanoidsTableWorker(TableDef tableDef) : ThingDefTableWorker(tableDef)
{
    protected override bool IsValidThingDef(Verse.ThingDef thingDef)
    {
        return thingDef is
        {
            race.IsMechanoid: true,
            IsCorpse: false
        }
        && thingDef.race?.mechEnabledWorkTypes?.Count > 0;
    }
}
