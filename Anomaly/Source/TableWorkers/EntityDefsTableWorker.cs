using Verse;

namespace Stats.Compat.Anomaly;

public sealed class EntityDefsTableWorker : ThingDefsTableWorker
{
    public EntityDefsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef is { race.IsAnomalyEntity: true, IsCorpse: false };
    }
}
