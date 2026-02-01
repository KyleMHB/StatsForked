using Stats.Objects.ThingDef;

namespace Stats.Objects.ThingDef.ColumnWorkers.Animal;

public sealed class Animal_IsPackAnimalColumnWorker : BooleanColumnWorker<VirtualThing>
{
    public Animal_IsPackAnimalColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetCellValue(VirtualThing thing)
    {
        return thing.Def.race?.packAnimal == true;
    }
}
