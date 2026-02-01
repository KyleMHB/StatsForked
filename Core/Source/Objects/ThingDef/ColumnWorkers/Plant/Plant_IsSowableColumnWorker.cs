using Stats.Objects.ThingDef;

namespace Stats.Objects.ThingDef.ColumnWorkers.Plant;

public sealed class Plant_IsSowableColumnWorker : BooleanColumnWorker<VirtualThing>
{
    public Plant_IsSowableColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetCellValue(VirtualThing thing)
    {
        return thing.Def.plant?.Sowable == true;
    }
}
