namespace Stats;

public sealed class Plant_GrowingTimeColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public Plant_GrowingTimeColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps == null)
        {
            return "";
        }

        return $"{plantProps.growDays:0.##} d";
    }
}
