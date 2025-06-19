namespace Stats;

public sealed class Plant_CanBePlantedUnderRoofColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public Plant_CanBePlantedUnderRoofColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(ThingAlike thing)
    {
        return thing.Def.plant?.interferesWithRoof == false;
    }
}
