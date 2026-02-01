using Stats.Objects.ThingDef;

namespace Stats.Objects.ThingDef.ColumnWorkers.Plant;

public sealed class Plant_IsTreeColumnWorker : BooleanColumnWorker<VirtualThing>
{
    public Plant_IsTreeColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetCellValue(VirtualThing thing)
    {
        return thing.Def.plant?.IsTree == true;
    }
}
