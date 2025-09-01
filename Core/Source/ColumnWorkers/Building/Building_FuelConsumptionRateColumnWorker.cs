namespace Stats;

public sealed class Building_FuelConsumptionRateColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Building_FuelConsumptionRateColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0/d")
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var refuelableCompProps = thing.Def.GetRefuelableCompProperties();

        if (refuelableCompProps == null)
        {
            return 0m;
        }

        // TODO: Difficulty scaling.
        return refuelableCompProps.fuelConsumptionRate.ToDecimal(1);
    }
}
