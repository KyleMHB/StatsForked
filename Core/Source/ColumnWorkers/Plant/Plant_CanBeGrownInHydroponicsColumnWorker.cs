namespace Stats;

public sealed class Plant_CanBeGrownInHydroponicsColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public Plant_CanBeGrownInHydroponicsColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(ThingAlike thing)
    {
        return thing.Def.plant?.sowTags.Contains("Hydroponic") == true;
    }
}
