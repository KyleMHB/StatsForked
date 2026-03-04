using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Bed;

public sealed class FitsLargeAnimalsColumnWorker(ColumnDef columnDef) : StaticColumnWorker<DefBasedObject,>
{
    public override ColumnDef Def => columnDef;

    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        if (thingDef.building is { bed_humanlike: false, bed_maxBodySize: > 0.55f })
        {
            return BooleanTableCell.True;
        }

        return BooleanTableCell.False;
    }

    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => BooleanTableCell.GetDescriptor(columnDef);
}
