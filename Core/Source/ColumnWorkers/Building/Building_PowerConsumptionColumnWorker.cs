namespace Stats;

public sealed class Building_PowerConsumptionColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Building_PowerConsumptionColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0 W")
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var powerCompProps = thing.Def.GetPowerCompProperties();

        if (powerCompProps == null || powerCompProps.PowerConsumption < 0f)
        {
            return 0m;
        }

        return powerCompProps.PowerConsumption.ToDecimal(0);
    }
}
