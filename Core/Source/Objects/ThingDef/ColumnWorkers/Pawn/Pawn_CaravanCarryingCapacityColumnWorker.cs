using RimWorld;
using Stats.Objects.ThingDef;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.Pawn;

public sealed class Pawn_CaravanCarryingCapacityColumnWorker : NumberColumnWorker<VirtualThing>
{
    public Pawn_CaravanCarryingCapacityColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0 kg")
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        var raceProps = thing.Def.race;

        if (raceProps != null)
        {
            return (raceProps.baseBodySize * MassUtility.MassCapacityPerBodySize).ToDecimal(0);
        }

        return 0m;
    }
}
