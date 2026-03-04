namespace Stats.ColumnWorkers.ThingDef.Animal;

public sealed class Animal_IsPackAnimalColumnWorker : StaticColumnWorker<DefBasedObject,>
{
    public override ColumnDef Def => columnDef;

    public Animal_IsPackAnimalColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetCellValue(VirtualThing thing)
    {
        return thing.Def.race?.packAnimal == true;
    }
}
