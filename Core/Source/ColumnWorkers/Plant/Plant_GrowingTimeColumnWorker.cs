namespace Stats;

public sealed class Plant_GrowingTimeColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Plant_GrowingTimeColumnWorker(ColumnDef columnDef) : base(columnDef, formatString: "0.0 d")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps?.growDays > 0f)
        {
            return plantProps.growDays.ToDecimal(1);
        }

        return 0m;
    }
}
