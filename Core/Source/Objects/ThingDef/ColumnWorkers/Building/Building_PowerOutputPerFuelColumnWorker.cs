using Stats;
using Stats.Objects.ThingDef;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.Building;

public sealed class Building_PowerOutputPerFuelColumnWorker : NumberColumnWorker<VirtualThing>
{
    public Building_PowerOutputPerFuelColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0 W/u")
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        var powerCompProps = thing.Def.GetPowerCompProperties();
        var refuelableCompProps = thing.Def.GetRefuelableCompProperties();

        if
        (
            powerCompProps == null
            || refuelableCompProps == null
            || refuelableCompProps.fuelConsumptionRate == 0f
        )
        {
            return 0m;
        }

        var powerOutput = powerCompProps.PowerConsumption * -1f;
        var fuelConsumptionRate = refuelableCompProps.fuelConsumptionRate;

        return (powerOutput / fuelConsumptionRate).ToDecimal(0);
    }
}
