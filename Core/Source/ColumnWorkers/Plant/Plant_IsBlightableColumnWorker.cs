namespace Stats;

public sealed class Plant_IsBlightableColumnWorker : BooleanColumnWorker<AbstractThing>
{
    public Plant_IsBlightableColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(AbstractThing thing)
    {
        return thing.Def.plant?.Blightable == true;
    }
}
