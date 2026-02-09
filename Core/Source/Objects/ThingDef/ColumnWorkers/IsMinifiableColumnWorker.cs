using Stats.ObjectTable;
using Stats.ObjectTable.Cells;

namespace Stats.Objects.ThingDef.ColumnWorkers;

public sealed class IsMinifiableColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        bool cellValue = thingDef.Minifiable;

        return new BooleanCell(cellValue);
    }
    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker) => BooleanCell.GetDescriptor(columnDef);
}
