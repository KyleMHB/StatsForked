using RimWorld;
using Stats.Extensions;
using Stats.TableCells;

namespace Stats.ColumnWorkers.ThingDef.EggLayer;

public sealed class EggsNutritionPerDayColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            CompProperties_EggLayer? eggLayerCompProps = thingDef.GetCompProperties<CompProperties_EggLayer>();

            if (eggLayerCompProps is { eggLayIntervalDays: > 0f })
            {
                Verse.ThingDef eggDef = eggLayerCompProps.GetAnyEggDef();
                float eggNutrition = eggDef.GetStatValuePerceived(StatDefOf.Nutrition);
                float eggsPerDay = eggLayerCompProps.eggCountRange.Average / eggLayerCompProps.eggLayIntervalDays;
                decimal cellValue = (eggsPerDay * eggNutrition).ToDecimal(2);

                return new NumberTableCell(cellValue, "0.00/d");
            }
        }

        return default;
    }
}
