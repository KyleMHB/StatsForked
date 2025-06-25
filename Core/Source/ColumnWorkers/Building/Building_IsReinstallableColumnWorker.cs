namespace Stats;

public sealed class Building_IsReinstallableColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public Building_IsReinstallableColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(ThingAlike thing)
    {
        return thing.Def.Minifiable;
    }
}
