using Stats.ObjectTable;
using Stats.ObjectTable.Cells;

namespace Stats.Objects.ThingDef.ColumnWorkers;

public sealed class IsMinifiableColumnWorker(ColumnDef columnDef) : StaticColumnWorker<DefBasedObject, BooleanCell>
{
    public override ColumnDef Def => columnDef;

    protected override BooleanCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            return new BooleanCell(thingDef.Minifiable);
        }

        return default;
    }

    //public override CellDescriptor GetCellDescriptor(TableWorker tableWorker) => BooleanCell.GetDescriptor(columnDef);
}
