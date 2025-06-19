namespace Stats;

public sealed class Animal_IsBlockedByFencesColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public Animal_IsBlockedByFencesColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(ThingAlike thing)
    {
        return thing.Def.race?.FenceBlocked == true;
    }
}
