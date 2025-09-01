namespace Stats;

public sealed class Animal_IsBlockedByFencesColumnWorker : BooleanColumnWorker<AbstractThing>
{
    public Animal_IsBlockedByFencesColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(AbstractThing thing)
    {
        return thing.Def.race?.FenceBlocked == true;
    }
}
