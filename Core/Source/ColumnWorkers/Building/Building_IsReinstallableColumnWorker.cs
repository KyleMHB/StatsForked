namespace Stats;

public sealed class Building_IsReinstallableColumnWorker : BooleanColumnWorker<AbstractThing>
{
    public Building_IsReinstallableColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(AbstractThing thing)
    {
        return thing.Def.Minifiable;
    }
}
