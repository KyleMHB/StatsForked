using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.EggLayer;

public sealed class EggsNutritionPerDayColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        CompProperties_EggLayer? eggLayerCompProps = thingDef.GetCompProperties<CompProperties_EggLayer>();

        if (eggLayerCompProps is { eggLayIntervalDays: > 0f })
        {
            Verse.ThingDef eggDef = eggLayerCompProps.GetAnyEggDef();
            float eggNutrition = eggDef.GetStatValuePerceived(StatDefOf.Nutrition);
            float eggsPerDay = eggLayerCompProps.eggCountRange.Average / eggLayerCompProps.eggLayIntervalDays;
            decimal cellValue = (eggsPerDay * eggNutrition).ToDecimal(2);

            return new NumberCell.Constant(cellValue, "0.00/d");
        }

        return NumberCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
