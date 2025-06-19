namespace Stats;

public sealed class Animal_IsPackAnimalColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public Animal_IsPackAnimalColumnWorker(ColumnDef columndef) : base(columndef, false)
    {
    }
    protected override bool GetValue(ThingAlike thing)
    {
        return thing.Def.race?.packAnimal == true;
    }
}
