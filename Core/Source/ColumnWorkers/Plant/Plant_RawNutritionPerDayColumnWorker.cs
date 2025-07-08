using RimWorld;

namespace Stats;

public sealed class Plant_RawNutritionPerDayColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Plant_RawNutritionPerDayColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.000/d")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps is { growDays: > 0f })
        {
            var nutrition = thing.Def.GetStatValuePerceived(StatDefOf.Nutrition);

            return (nutrition / plantProps.growDays).ToDecimal(3);
        }

        return 0m;
    }
}
