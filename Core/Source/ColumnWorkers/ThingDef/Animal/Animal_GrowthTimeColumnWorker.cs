using RimWorld;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.ColumnWorkers.ThingDef.Animal;

public sealed class Animal_GrowthTimeColumnWorker : NumberColumnWorker<VirtualThing>
{
    public Animal_GrowthTimeColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0 d")
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        var raceProps = thing.Def.race;

        if (raceProps != null)
        {
            return AnimalProductionUtility.DaysToAdulthood(thing.Def).ToDecimal(0);
        }

        return 0m;
    }
}
