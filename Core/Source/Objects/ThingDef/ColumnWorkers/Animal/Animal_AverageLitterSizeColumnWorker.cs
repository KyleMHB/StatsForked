using RimWorld;
using Stats.Objects.ThingDef;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.Animal;

public sealed class Animal_AverageLitterSizeColumnWorker : NumberColumnWorker<VirtualThing>
{
    public Animal_AverageLitterSizeColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0")
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        if (thing.Def.race != null)
        {
            return AnimalProductionUtility.OffspringRange(thing.Def).Average.ToDecimal(1);
        }

        return 0m;
    }
}
