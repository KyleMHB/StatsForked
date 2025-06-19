namespace Stats;

public sealed class Building_FuelCapacityColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Building_FuelCapacityColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var refuelableCompProps = thing.Def.GetRefuelableCompProperties();

        if (refuelableCompProps == null)
        {
            return 0m;
        }

        return refuelableCompProps.fuelCapacity.ToDecimal(0);
    }
}
