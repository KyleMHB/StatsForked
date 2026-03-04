using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Plant;

public sealed class IsDestroyedOnHarvestColumnWorker(ColumnDef columnDef) : StaticColumnWorker<DefBasedObject,>
{
    public override ColumnDef Def => columnDef;

    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
        }
        if (thingDef.plant?.HarvestDestroys == true)
        {
            return BooleanTableCell.True;
        }

        return BooleanTableCell.False;
    }

    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => BooleanTableCell.GetDescriptor(columnDef);
}
