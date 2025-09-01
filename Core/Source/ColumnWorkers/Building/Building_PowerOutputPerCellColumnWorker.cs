namespace Stats;

public sealed class Building_PowerOutputPerCellColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Building_PowerOutputPerCellColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0 W/c")
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var powerCompProps = thing.Def.GetPowerCompProperties();

        if (powerCompProps == null)
        {
            return 0m;
        }

        return powerCompProps.PowerConsumption.ToDecimal(0) * -1m / thing.Def.size.Area;
    }
}
