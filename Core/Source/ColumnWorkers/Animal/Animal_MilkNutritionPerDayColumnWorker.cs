using RimWorld;

namespace Stats;

public sealed class Animal_MilkNutritionPerDayColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Animal_MilkNutritionPerDayColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.00/d")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var milkableCompProps = thing.Def.GetCompProperties<CompProperties_Milkable>();

        if (milkableCompProps is { milkDef: not null, milkIntervalDays: > 0 })
        {
            var milkNutrition = milkableCompProps.milkDef.GetStatValuePerceived(StatDefOf.Nutrition);
            var milkPerDay = (float)milkableCompProps.milkAmount / milkableCompProps.milkIntervalDays;

            return (milkPerDay * milkNutrition).ToDecimal(2);
        }

        return 0m;
    }
}
