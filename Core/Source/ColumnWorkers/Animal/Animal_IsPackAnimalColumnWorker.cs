namespace Stats;

public sealed class Animal_IsPackAnimalColumnWorker : BooleanColumnWorker<AbstractThing>
{
    public Animal_IsPackAnimalColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(AbstractThing thing)
    {
        return thing.Def.race?.packAnimal == true;
    }
}
