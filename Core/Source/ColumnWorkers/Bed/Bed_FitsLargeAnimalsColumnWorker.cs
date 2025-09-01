namespace Stats;

public sealed class Bed_FitsLargeAnimalsColumnWorker : BooleanColumnWorker<AbstractThing>
{
    public Bed_FitsLargeAnimalsColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(AbstractThing thing)
    {
        var buildingProps = thing.Def.building;

        if (buildingProps == null)
        {
            return false;
        }

        return buildingProps.bed_humanlike == false && buildingProps.bed_maxBodySize > 0.55f;
    }
}
