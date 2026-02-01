using Stats.Objects.ThingDef;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.Animal;

public sealed class Animal_NuzzleIntervalColumnWorker : NumberColumnWorker<VirtualThing>
{
    public Animal_NuzzleIntervalColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0 h")
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        var raceProps = thing.Def.race;

        if (raceProps != null)
        {
            return raceProps.nuzzleMtbHours.ToDecimal(1);
        }

        return 0m;
    }
}
