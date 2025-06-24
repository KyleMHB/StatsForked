using RimWorld;

namespace Stats;

public sealed class Animal_ProductsNutritionPerDayColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Animal_ProductsNutritionPerDayColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.00/d")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var milkNutritionPerDay = 0f;
        var milkableCompProps = thing.Def.GetCompProperties<CompProperties_Milkable>();

        if (milkableCompProps is { milkDef: not null, milkAmount: > 0, milkIntervalDays: > 0 })
        {
            var statRequest = StatRequest.For(milkableCompProps.milkDef, null);
            var nutritionStatWorker = StatDefOf.Nutrition.Worker;

            if (nutritionStatWorker.ShouldShowFor(statRequest))
            {
                var milkNutrition = nutritionStatWorker.GetValue(statRequest);
                var milkPerDay = (float)milkableCompProps.milkAmount / milkableCompProps.milkIntervalDays;

                milkNutritionPerDay = milkPerDay * milkNutrition;
            }
        }

        var eggsNutritionPerDay = 0f;
        var eggLayerCompProps = thing.Def.GetCompProperties<CompProperties_EggLayer>();

        if (eggLayerCompProps is { eggLayIntervalDays: > 0 })
        {
            var eggType = eggLayerCompProps.eggUnfertilizedDef ?? eggLayerCompProps.eggFertilizedDef;
            var statRequest = StatRequest.For(eggType, null);
            var nutritionStatWorker = StatDefOf.Nutrition.Worker;

            if (nutritionStatWorker.ShouldShowFor(statRequest))
            {
                var eggNutrition = nutritionStatWorker.GetValue(statRequest);
                var eggsPerDay = eggLayerCompProps.eggCountRange.Average / eggLayerCompProps.eggLayIntervalDays;

                eggsNutritionPerDay = eggsPerDay * eggNutrition;
            }
        }

        return (milkNutritionPerDay + eggsNutritionPerDay).ToDecimal(2);
    }
}
