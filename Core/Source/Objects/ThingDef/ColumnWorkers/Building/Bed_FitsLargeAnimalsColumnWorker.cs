using Stats.Objects.ThingDef;

namespace Stats.Objects.ThingDef.ColumnWorkers.Building;

public sealed class Bed_FitsLargeAnimalsColumnWorker : BooleanColumnWorker<VirtualThing>
{
    public Bed_FitsLargeAnimalsColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetCellValue(VirtualThing thing)
    {
        var buildingProps = thing.Def.building;

        if (buildingProps == null)
        {
            return false;
        }

        return buildingProps.bed_humanlike == false && buildingProps.bed_maxBodySize > 0.55f;
    }
}
