using Stats;
using Stats.Objects.ThingDef;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.Building;

public sealed class Building_DaysPerRefuelColumnWorker : NumberColumnWorker<VirtualThing>
{
    public Building_DaysPerRefuelColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0 d")
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        var refuelableCompProps = thing.Def.GetRefuelableCompProperties();

        if (refuelableCompProps == null || refuelableCompProps.fuelConsumptionRate == 0f)
        {
            return 0m;
        }

        return (refuelableCompProps.fuelCapacity / refuelableCompProps.fuelConsumptionRate).ToDecimal(1);
    }
}
