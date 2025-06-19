namespace Stats;

public sealed class Plant_LifeSpanColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public Plant_LifeSpanColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps == null)
        {
            return "";
        }

        return $"{plantProps.LifespanDays:0.##} d";
    }
}
