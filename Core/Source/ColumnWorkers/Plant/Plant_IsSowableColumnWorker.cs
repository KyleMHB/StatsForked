namespace Stats;

public sealed class Plant_IsSowableColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public Plant_IsSowableColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(ThingAlike thing)
    {
        return thing.Def.plant?.Sowable == true;
    }
}
