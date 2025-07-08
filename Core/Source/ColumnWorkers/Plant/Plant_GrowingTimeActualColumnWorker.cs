namespace Stats;

public sealed class Plant_GrowingTimeActualColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Plant_GrowingTimeActualColumnWorker(ColumnDef columnDef) : base(columnDef, formatString: "0.0 d")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps?.growDays > 0f)
        {
            return plantProps.GetGrowDaysActual().ToDecimal(1);
        }

        return 0m;
    }
}
