namespace Stats;

public sealed class Building_DaysPerRefuelColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Building_DaysPerRefuelColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0 d")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var refuelableCompProps = thing.Def.GetRefuelableCompProperties();

        if (refuelableCompProps == null || refuelableCompProps.fuelConsumptionRate == 0f)
        {
            return 0m;
        }

        return (refuelableCompProps.fuelCapacity / refuelableCompProps.fuelConsumptionRate).ToDecimal(1);
    }
}
