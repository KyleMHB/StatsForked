namespace Stats;

public sealed class Plant_IsTreeColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public Plant_IsTreeColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(ThingAlike thing)
    {
        return thing.Def.plant?.IsTree == true;
    }
}
