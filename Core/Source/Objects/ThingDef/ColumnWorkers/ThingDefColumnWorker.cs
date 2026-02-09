using Stats.ObjectTable;
using Stats.ObjectTable.Cells;

namespace Stats.Objects.ThingDef.ColumnWorkers;

public abstract class ThingDefColumnWorker :
    IColumnWorker<Verse.ThingDef>,
    IColumnWorker<VirtualThing>,
    IColumnWorker<Verse.Thing>
{
    public Cell MakeCell(Verse.Thing thing) => MakeCell(thing.def);
    public Cell MakeCell(VirtualThing thing) => MakeCell(thing.Def);
    public abstract Cell MakeCell(Verse.ThingDef thingDef);
    public abstract CellDescriptor GetCellDescriptor(TableWorker tableWorker);
}
