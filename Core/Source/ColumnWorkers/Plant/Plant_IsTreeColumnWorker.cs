namespace Stats;

public sealed class Plant_IsTreeColumnWorker : BooleanColumnWorker<AbstractThing>
{
    public Plant_IsTreeColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(AbstractThing thing)
    {
        return thing.Def.plant?.IsTree == true;
    }
}
