using RimWorld;

namespace Stats;

public sealed class Animal_EggsNutritionPerDayColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Animal_EggsNutritionPerDayColumnWorker(ColumnDef columnDef) : base(columnDef, formatString: "0.00/d")
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var eggLayerCompProps = thing.Def.GetCompProperties<CompProperties_EggLayer>();

        if (eggLayerCompProps is { eggLayIntervalDays: > 0f })
        {
            var eggDef = eggLayerCompProps.GetAnyEggDef();
            var eggNutrition = eggDef.GetStatValuePerceived(StatDefOf.Nutrition);
            var eggsPerDay = eggLayerCompProps.eggCountRange.Average / eggLayerCompProps.eggLayIntervalDays;

            return (eggsPerDay * eggNutrition).ToDecimal(2);
        }

        return 0m;
    }
}
