using RimWorld;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;

namespace Stats.Objects.ThingDef.ColumnWorkers.Plant;

public sealed class IsBlightableColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        PlantProperties? plantProps = thingDef.plant;

        if (plantProps != null)
        {
            return new BooleanCell(plantProps.Blightable);
        }

        return BooleanCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker) => BooleanCell.GetDescriptor(columnDef);
}
