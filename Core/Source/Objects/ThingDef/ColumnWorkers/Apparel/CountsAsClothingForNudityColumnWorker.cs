using Stats.Objects.ThingDef;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.Apparel;

public sealed class CountsAsClothingForNudityColumnWorker : BooleanColumnWorker<VirtualThing>
{
    public CountsAsClothingForNudityColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override bool GetCellValue(VirtualThing thing)
    {
        return thing.Def.apparel?.countsAsClothingForNudity ?? false;
    }
}
