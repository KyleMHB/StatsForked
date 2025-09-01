namespace Stats;

public sealed class Plant_CanBeGrownInHydroponicsColumnWorker : BooleanColumnWorker<AbstractThing>
{
    public Plant_CanBeGrownInHydroponicsColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(AbstractThing thing)
    {
        return thing.Def.plant?.sowTags.Contains("Hydroponic") == true;
    }
}
