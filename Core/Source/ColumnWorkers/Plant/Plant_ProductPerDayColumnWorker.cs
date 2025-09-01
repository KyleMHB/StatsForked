namespace Stats;

public sealed class Plant_ProductPerDayColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Plant_ProductPerDayColumnWorker(ColumnDef columnDef) : base(columnDef, formatString: "0.00/d")
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps is { growDays: > 0f })
        {
            return (plantProps.harvestYield / plantProps.GetGrowDaysActual()).ToDecimal(2);
        }

        return 0m;
    }
}
