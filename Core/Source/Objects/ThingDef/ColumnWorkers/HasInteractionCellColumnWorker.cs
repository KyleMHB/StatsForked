using Stats.ObjectTable.Cells;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers;
// IsMannedColumnWorker
public sealed class HasInteractionCellColumnWorker(ColumnDef columnDef) :
    BooleanColumnWorker<Verse.ThingDef>(columnDef),
    IColumnWorker<VirtualThing>,
    IColumnWorker<Verse.Thing>
{
    public Cell GetCell(VirtualThing thing) => GetCell(thing.Def);
    public Cell GetCell(Verse.Thing thing) => GetCell(thing.def);
    protected override CellValueSource<bool> GetCellValueSource(Verse.ThingDef thingDef)
    {
        bool cellValue = thingDef.hasInteractionCell;

        return () => cellValue;
    }
}
