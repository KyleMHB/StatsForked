using RimWorld;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;

namespace Stats.Objects.ThingDef.ColumnWorkers.Plant;

public sealed class RawNutritionPerDayColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell GetCell(Verse.ThingDef thingDef)
    {
        PlantProperties? plantProps = thingDef.plant;

        if (plantProps is { growDays: > 0f })
        {
            float nutrition = thingDef.GetStatValuePerceived(StatDefOf.Nutrition);
            decimal cellValue = (nutrition / plantProps.GetGrowDaysActual()).ToDecimal(3);

            return new NumberCell(cellValue, "0.000/d");
        }

        return NumberCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
