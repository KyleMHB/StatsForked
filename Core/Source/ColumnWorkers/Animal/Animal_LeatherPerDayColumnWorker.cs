using RimWorld;

namespace Stats;

public sealed class Animal_LeatherPerDayColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Animal_LeatherPerDayColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0/d")
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
                var leatherAmount = thing.Def.GetStatValuePerceived(StatDefOf.LeatherAmount);

                return (leatherAmount / growthTime).ToDecimal(1);
            }
        }

        return 0m;
    }
}
