using Stats.Objects.ThingDef;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.Plant;

public sealed class Plant_MinGrowingSkillToSowColumnWorker : NumberColumnWorker<VirtualThing>
{
    public Plant_MinGrowingSkillToSowColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        return thing.Def.plant?.sowMinSkill ?? 0m;
    }
}
