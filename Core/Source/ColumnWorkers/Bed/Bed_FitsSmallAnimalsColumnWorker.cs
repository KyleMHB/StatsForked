namespace Stats;

public sealed class Bed_FitsSmallAnimalsColumnWorker : BooleanColumnWorker<AbstractThing>
{
    public Bed_FitsSmallAnimalsColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(AbstractThing thing)
    {
        return thing.Def.building?.bed_humanlike == false;
    }
}
