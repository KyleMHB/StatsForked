namespace Stats;

public sealed class Plant_MinGrowingSkillToSowColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Plant_MinGrowingSkillToSowColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        return thing.Def.plant?.sowMinSkill ?? 0m;
    }
}
