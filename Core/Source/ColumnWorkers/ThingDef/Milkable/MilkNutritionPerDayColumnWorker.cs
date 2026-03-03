using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Milkable;

public sealed class MilkNutritionPerDayColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        CompProperties_Milkable? milkableCompProps = thingDef.GetCompProperties<CompProperties_Milkable>();

        if (milkableCompProps is { milkDef: not null, milkIntervalDays: > 0 })
        {
            float milkNutrition = milkableCompProps.milkDef.GetStatValuePerceived(StatDefOf.Nutrition);
            float milkPerDay = (float)milkableCompProps.milkAmount / milkableCompProps.milkIntervalDays;
            decimal cellValue = (milkPerDay * milkNutrition).ToDecimal(2);

            return new NumberCell.Constant(cellValue, "0.00/d");
        }

        return NumberCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
