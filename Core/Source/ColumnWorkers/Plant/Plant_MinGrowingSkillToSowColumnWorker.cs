namespace Stats;

public sealed class Plant_MinGrowingSkillToSowColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Plant_MinGrowingSkillToSowColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        return thing.Def.plant?.sowMinSkill ?? 0m;
    }
}
