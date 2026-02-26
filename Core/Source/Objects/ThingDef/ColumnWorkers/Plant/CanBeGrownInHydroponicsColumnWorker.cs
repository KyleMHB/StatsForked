using Stats.ObjectTable;
using Stats.ObjectTable.Cells;

namespace Stats.Objects.ThingDef.ColumnWorkers.Plant;

public sealed class CanBeGrownInHydroponicsColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        if (thingDef.plant?.sowTags.Contains("Hydroponic") == true)
        {
            return BooleanCell.True;
        }

        return BooleanCell.False;
    }

    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker) => BooleanCell.GetDescriptor(columnDef);
}
