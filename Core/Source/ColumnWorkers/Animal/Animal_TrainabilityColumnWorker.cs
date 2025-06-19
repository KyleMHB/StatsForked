using Verse;

namespace Stats;

public sealed class Animal_TrainabilityColumnWorker : DefColumnWorker<ThingAlike, TrainabilityDef?>
{
    public Animal_TrainabilityColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override TrainabilityDef? GetValue(ThingAlike thing)
    {
        return thing.Def.race?.trainability;
    }
}
