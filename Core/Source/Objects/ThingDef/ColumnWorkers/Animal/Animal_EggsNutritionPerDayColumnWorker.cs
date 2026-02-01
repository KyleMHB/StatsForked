using RimWorld;
using Stats.Objects.ThingDef;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.Animal;

public sealed class Animal_EggsNutritionPerDayColumnWorker : NumberColumnWorker<VirtualThing>
{
    public Animal_EggsNutritionPerDayColumnWorker(ColumnDef columnDef) : base(columnDef, formatString: "0.00/d")
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
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
