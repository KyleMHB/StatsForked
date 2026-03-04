using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Apparel;

public sealed class CountsAsClothingForNudityColumnWorker(ColumnDef columnDef) : StaticColumnWorker<DefBasedObject,>
{
    public override ColumnDef Def => columnDef;

    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        if (thingDef.apparel?.countsAsClothingForNudity == true)
        {
            return BooleanTableCell.True;
        }

        return BooleanTableCell.False;
    }

    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => BooleanTableCell.GetDescriptor(columnDef);
}
