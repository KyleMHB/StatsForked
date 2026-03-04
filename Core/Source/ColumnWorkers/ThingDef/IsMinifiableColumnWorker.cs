using Stats.TableCells;

namespace Stats.ColumnWorkers.ThingDef;

public sealed class IsMinifiableColumnWorker(ColumnDef columnDef) : StaticColumnWorker<DefBasedObject, BooleanTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override BooleanTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            return new BooleanTableCell(thingDef.Minifiable);
        }

        return default;
    }

    //public override CellDescriptor GetCellDescriptor(TableWorker tableWorker) => BooleanCell.GetDescriptor(columnDef);
}
