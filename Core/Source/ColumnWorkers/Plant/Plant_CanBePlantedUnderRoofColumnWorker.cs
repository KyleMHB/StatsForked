namespace Stats;

public sealed class Plant_CanBePlantedUnderRoofColumnWorker : BooleanColumnWorker<AbstractThing>
{
    public Plant_CanBePlantedUnderRoofColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(AbstractThing thing)
    {
        return thing.Def.plant?.interferesWithRoof == false;
    }
}
