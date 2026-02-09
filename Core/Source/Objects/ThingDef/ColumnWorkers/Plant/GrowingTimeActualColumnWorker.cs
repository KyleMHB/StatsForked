using RimWorld;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;

namespace Stats.Objects.ThingDef.ColumnWorkers.Plant;

public sealed class GrowingTimeActualColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        PlantProperties? plantProps = thingDef.plant;

        if (plantProps?.growDays > 0f)
        {
            decimal cellValue = plantProps.GetGrowDaysActual().ToDecimal(1);

            return new NumberCell(cellValue, "0.0 d");
        }

        return NumberCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
