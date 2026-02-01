using Stats.Objects.ThingDef;

namespace Stats.Objects.ThingDef.ColumnWorkers.Building;

public sealed class Building_IsReinstallableColumnWorker : BooleanColumnWorker<VirtualThing>
{
    public Building_IsReinstallableColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetCellValue(VirtualThing thing)
    {
        return thing.Def.Minifiable;
    }
}
