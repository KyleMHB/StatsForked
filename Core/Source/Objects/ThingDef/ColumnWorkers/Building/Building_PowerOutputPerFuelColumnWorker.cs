using RimWorld;
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
        CompProperties_Power? powerCompProps = thing.Def.GetCompProperties<CompProperties_Power>();
        CompProperties_Refuelable? refuelableCompProps = thing.Def.GetCompProperties<CompProperties_Refuelable>();

        if (powerCompProps != null && refuelableCompProps is { fuelConsumptionRate: not 0f })
        {
            var powerOutput = powerCompProps.PowerConsumption * -1f;
            var fuelConsumptionRate = refuelableCompProps.fuelConsumptionRate;

            return (powerOutput / fuelConsumptionRate).ToDecimal(0);
        }

        return 0m;
    }
}
