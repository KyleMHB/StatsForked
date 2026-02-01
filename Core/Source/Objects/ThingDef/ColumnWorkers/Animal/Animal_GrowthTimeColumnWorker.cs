using RimWorld;
using Stats.Objects.ThingDef;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.Animal;

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
