using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Bed;

public sealed class FitsSmallAnimalsColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        if (thingDef.building?.bed_humanlike == false)
        {
            return BooleanTableCell.True;
        }

        return BooleanTableCell.False;
    }

    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => BooleanTableCell.GetDescriptor(columnDef);
}
