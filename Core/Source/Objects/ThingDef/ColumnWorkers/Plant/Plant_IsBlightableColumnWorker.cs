using Stats.Objects.ThingDef;

namespace Stats.Objects.ThingDef.ColumnWorkers.Plant;

public sealed class Plant_IsBlightableColumnWorker : BooleanColumnWorker<VirtualThing>
{
    public Plant_IsBlightableColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetCellValue(VirtualThing thing)
    {
        return thing.Def.plant?.Blightable == true;
    }
}
