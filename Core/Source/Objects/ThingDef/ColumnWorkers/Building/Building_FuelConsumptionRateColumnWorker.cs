using Stats;
using Stats.Objects.ThingDef;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.Building;

public sealed class Building_FuelConsumptionRateColumnWorker : NumberColumnWorker<VirtualThing>
{
    public Building_FuelConsumptionRateColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0/d")
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
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
