namespace Stats;

public sealed class Plant_IsBlightableColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public Plant_IsBlightableColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(ThingAlike thing)
    {
        return thing.Def.plant?.Blightable == true;
    }
}
