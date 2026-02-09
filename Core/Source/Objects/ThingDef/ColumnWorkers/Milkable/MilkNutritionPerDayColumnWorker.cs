using RimWorld;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;

namespace Stats.Objects.ThingDef.ColumnWorkers.Milkable;

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

            return new NumberCell(cellValue, "0.00/d");
        }

        return NumberCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
