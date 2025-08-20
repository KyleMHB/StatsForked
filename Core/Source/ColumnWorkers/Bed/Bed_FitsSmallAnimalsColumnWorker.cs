namespace Stats;

public sealed class Bed_FitsSmallAnimalsColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public Bed_FitsSmallAnimalsColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(ThingAlike thing)
    {
        return thing.Def.building?.bed_humanlike == false;
    }
}
