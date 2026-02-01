using Stats.ObjectTable;
using Stats.ObjectTable.Cells;

namespace Stats.Objects.ThingDef.ColumnWorkers;

public sealed class IsMinifiableColumnWorker(ColumnDef columnDef) :
    IColumnWorker<Verse.ThingDef>,
    IColumnWorker<VirtualThing>,
    IColumnWorker<Verse.Thing>
{
    public Cell GetCell(Verse.Thing thing) => GetCell(thing.def);
    public Cell GetCell(VirtualThing thing) => GetCell(thing.Def);
    public Cell GetCell(Verse.ThingDef thingDef)
    {
        bool cellValue = thingDef.Minifiable;

        return new BooleanCell(cellValue);
    }
    public CellDescriptor GetCellDescriptor() => BooleanCell.GetDescriptor(columnDef);
}
