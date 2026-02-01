using Stats.Objects.ThingDef;

namespace Stats.Objects.ThingDef.ColumnWorkers.Animal;

public sealed class Animal_IsBlockedByFencesColumnWorker : BooleanColumnWorker<VirtualThing>
{
    public Animal_IsBlockedByFencesColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetCellValue(VirtualThing thing)
    {
        return thing.Def.race?.FenceBlocked == true;
    }
}
