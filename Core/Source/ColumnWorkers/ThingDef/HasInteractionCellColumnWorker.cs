using Stats.TableCells;

namespace Stats.ColumnWorkers.ThingDef;

public sealed class HasInteractionCellColumnWorker(ColumnDef columnDef) : StaticColumnWorker<DefBasedObject, BooleanTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override BooleanTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            return new BooleanTableCell(thingDef.hasInteractionCell);
        }

        return default;
    }

    //public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => BooleanTableCell.GetDescriptor(columnDef);
}
