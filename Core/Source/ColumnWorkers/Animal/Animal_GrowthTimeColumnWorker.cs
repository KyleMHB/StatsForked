using RimWorld;

namespace Stats;

public sealed class Animal_GrowthTimeColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Animal_GrowthTimeColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0 d")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var raceProps = thing.Def.race;

        if (raceProps != null)
        {
            var growthTime = AnimalProductionUtility.DaysToAdulthood(thing.Def);

            if (growthTime > 0f)
            {
                return growthTime.ToDecimal(0);
            }
        }

        return 0m;
    }
}
