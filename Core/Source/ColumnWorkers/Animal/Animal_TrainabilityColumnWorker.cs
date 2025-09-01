using Verse;

namespace Stats;

public sealed class Animal_TrainabilityColumnWorker : DefColumnWorker<AbstractThing, TrainabilityDef?>
{
    public Animal_TrainabilityColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override TrainabilityDef? GetValue(AbstractThing thing)
    {
        return thing.Def.race?.trainability;
    }
}
