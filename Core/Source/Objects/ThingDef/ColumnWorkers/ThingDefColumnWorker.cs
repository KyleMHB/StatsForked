using Stats.ObjectTable;

namespace Stats.Objects.ThingDef.ColumnWorkers;

public abstract class ThingDefColumnWorker :
    IColumnWorker<Verse.ThingDef>,
    IColumnWorker<VirtualThing>,
    IColumnWorker<Verse.Thing>
{
    public Cell GetCell(Verse.Thing thing) => GetCell(thing.def);
    public Cell GetCell(VirtualThing thing) => GetCell(thing.Def);
    public abstract Cell GetCell(Verse.ThingDef thingDef);
    public abstract CellDescriptor GetCellDescriptor();
}
