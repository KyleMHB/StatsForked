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

        if (milkableCompProps is { milkDef: not null, milkIntervalDays: > 0 })
        {
            var milkNutrition = milkableCompProps.milkDef.GetStatValuePerceived(StatDefOf.Nutrition);
            var milkPerDay = (float)milkableCompProps.milkAmount / milkableCompProps.milkIntervalDays;

            milkNutritionPerDay = milkPerDay * milkNutrition;
        }

        var eggsNutritionPerDay = 0f;
        var eggLayerCompProps = thing.Def.GetCompProperties<CompProperties_EggLayer>();

        if (eggLayerCompProps is { eggLayIntervalDays: > 0 })
        {
            var eggDef = eggLayerCompProps.GetAnyEggDef();
            var eggNutrition = eggDef.GetStatValuePerceived(StatDefOf.Nutrition);
            var eggsPerDay = eggLayerCompProps.eggCountRange.Average / eggLayerCompProps.eggLayIntervalDays;

            eggsNutritionPerDay = eggsPerDay * eggNutrition;
        }

        return (milkNutritionPerDay + eggsNutritionPerDay).ToDecimal(2);
    }
}
