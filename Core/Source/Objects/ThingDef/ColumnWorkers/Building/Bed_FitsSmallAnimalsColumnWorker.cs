using Stats.Objects.ThingDef;

namespace Stats.Objects.ThingDef.ColumnWorkers.Building;

public sealed class Bed_FitsSmallAnimalsColumnWorker : BooleanColumnWorker<VirtualThing>
{
    public Bed_FitsSmallAnimalsColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetCellValue(VirtualThing thing)
    {
        return thing.Def.building?.bed_humanlike == false;
    }
}
