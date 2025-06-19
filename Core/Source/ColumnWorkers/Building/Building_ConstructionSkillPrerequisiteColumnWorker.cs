namespace Stats;

public sealed class Building_ConstructionSkillPrerequisiteColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Building_ConstructionSkillPrerequisiteColumnWorker(ColumnDef columndef) : base(columndef, false)
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        if (thing.Def.BuildableByPlayer == false)
        {
            return 0m;
        }

        return thing.Def.constructionSkillPrerequisite;
    }
}
