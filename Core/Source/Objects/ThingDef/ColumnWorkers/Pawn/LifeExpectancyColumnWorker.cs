using Stats.Objects.ThingDef;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.Pawn;

public sealed class LifeExpectancyColumnWorker : NumberColumnWorker<VirtualThing>
{
    public LifeExpectancyColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0 y")
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        return thing.Def.race?.lifeExpectancy.ToDecimal(0) ?? 0m;
    }
}
