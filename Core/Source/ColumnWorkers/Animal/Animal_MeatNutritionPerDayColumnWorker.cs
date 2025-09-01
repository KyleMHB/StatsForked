using RimWorld;

namespace Stats;

public sealed class Animal_MeatNutritionPerDayColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Animal_MeatNutritionPerDayColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.00/d")
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var raceProps = thing.Def.race;

        if (raceProps is { meatDef: not null })
        {
            var growthTime = AnimalProductionUtility.DaysToAdulthood(thing.Def);

            if (growthTime > 0f)
            {
                var meatNutrition = thing.Def.race.meatDef.GetStatValuePerceived(StatDefOf.Nutrition);
                var meatAmount = AnimalProductionUtility.AdultMeatAmount(thing.Def);
                var meatPerDay = meatAmount / growthTime;

                return (meatPerDay * meatNutrition).ToDecimal(2);
            }
        }

        return 0m;
    }
}
