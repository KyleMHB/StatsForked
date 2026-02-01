using Stats.Objects.ThingDef;

namespace Stats.Objects.ThingDef.ColumnWorkers.Plant;

public sealed class Plant_CanBeGrownInHydroponicsColumnWorker : BooleanColumnWorker<VirtualThing>
{
    public Plant_CanBeGrownInHydroponicsColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetCellValue(VirtualThing thing)
    {
        return thing.Def.plant?.sowTags.Contains("Hydroponic") == true;
    }
}
