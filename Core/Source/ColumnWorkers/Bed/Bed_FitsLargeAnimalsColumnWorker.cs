namespace Stats;

public sealed class Bed_FitsLargeAnimalsColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public Bed_FitsLargeAnimalsColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(ThingAlike thing)
    {
        var buildingProps = thing.Def.building;

        if (buildingProps == null)
        {
            return false;
        }

        return buildingProps.bed_humanlike == false && buildingProps.bed_maxBodySize > 0.55f;
    }
}
