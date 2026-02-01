using Stats.Objects.ThingDef;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.Animal;

public sealed class Animal_TrainabilityColumnWorker : DefColumnWorker<VirtualThing, TrainabilityDef?>
{
    public Animal_TrainabilityColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override TrainabilityDef? GetValue(VirtualThing thing)
    {
        return thing.Def.race?.trainability;
    }
}
