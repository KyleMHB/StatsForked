using RimWorld;

namespace Stats;

public sealed class Animal_GrowthTimeColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Animal_GrowthTimeColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0 d")
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var raceProps = thing.Def.race;

        if (raceProps != null)
        {
            return AnimalProductionUtility.DaysToAdulthood(thing.Def).ToDecimal(0);
        }

        return 0m;
    }
}
