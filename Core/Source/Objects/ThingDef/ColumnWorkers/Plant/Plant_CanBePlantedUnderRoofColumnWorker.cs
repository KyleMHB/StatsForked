using Stats.Objects.ThingDef;

namespace Stats.Objects.ThingDef.ColumnWorkers.Plant;

public sealed class Plant_CanBePlantedUnderRoofColumnWorker : BooleanColumnWorker<VirtualThing>
{
    public Plant_CanBePlantedUnderRoofColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetCellValue(VirtualThing thing)
    {
        return thing.Def.plant?.interferesWithRoof == false;
    }
}
