using Stats.ObjectTable;
using Stats.ObjectTable.Cells;

namespace Stats.Objects.ThingDef.ColumnWorkers.Bed;

public sealed class FitsLargeAnimalsColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        if (thingDef.building is { bed_humanlike: false, bed_maxBodySize: > 0.55f })
        {
            return BooleanCell.True;
        }

        return BooleanCell.False;
    }

    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker) => BooleanCell.GetDescriptor(columnDef);
}
