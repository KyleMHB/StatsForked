namespace Stats.ColumnWorkers.ThingDef.Animal;

public sealed class Animal_IsBlockedByFencesColumnWorker : StaticColumnWorker<DefBasedObject,>
{
    public override ColumnDef Def => columnDef;

    public Animal_IsBlockedByFencesColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetCellValue(VirtualThing thing)
    {
        return thing.Def.race?.FenceBlocked == true;
    }
}
