using RimWorld;
using Stats.TableCells;

namespace Stats.ColumnWorkers.ThingDef.Animal;

public sealed class ProductsNutritionPerDayColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            float milkNutritionPerDay = 0f;
            CompProperties_Milkable? milkableCompProps = thingDef.GetCompProperties<CompProperties_Milkable>();

            if (milkableCompProps is { milkDef: not null, milkIntervalDays: > 0 })
            {
                float milkNutrition = milkableCompProps.milkDef.GetStatValuePerceived(StatDefOf.Nutrition);
                float milkPerDay = (float)milkableCompProps.milkAmount / milkableCompProps.milkIntervalDays;

                milkNutritionPerDay = milkPerDay * milkNutrition;
            }

            float eggsNutritionPerDay = 0f;
            CompProperties_EggLayer? eggLayerCompProps = thingDef.GetCompProperties<CompProperties_EggLayer>();

            if (eggLayerCompProps is { eggLayIntervalDays: > 0 })
            {
                Verse.ThingDef eggDef = eggLayerCompProps.GetAnyEggDef();
                float eggNutrition = eggDef.GetStatValuePerceived(StatDefOf.Nutrition);
                float eggsPerDay = eggLayerCompProps.eggCountRange.Average / eggLayerCompProps.eggLayIntervalDays;

                eggsNutritionPerDay = eggsPerDay * eggNutrition;
            }

            decimal cellValue = (milkNutritionPerDay + eggsNutritionPerDay).ToDecimal(2);

            return new NumberTableCell(cellValue, "0.00/d");
        }

        return default;
    }
}
