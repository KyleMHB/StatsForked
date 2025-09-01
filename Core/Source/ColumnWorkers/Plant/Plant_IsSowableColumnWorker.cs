namespace Stats;

public sealed class Plant_IsSowableColumnWorker : BooleanColumnWorker<AbstractThing>
{
    public Plant_IsSowableColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(AbstractThing thing)
    {
        return thing.Def.plant?.Sowable == true;
    }
}
