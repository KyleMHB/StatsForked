using Stats;
using Stats.Objects.ThingDef;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.Building;

public sealed class Building_PowerConsumptionColumnWorker : NumberColumnWorker<VirtualThing>
{
    public Building_PowerConsumptionColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0 W")
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        var powerCompProps = thing.Def.GetPowerCompProperties();

        if (powerCompProps == null || powerCompProps.PowerConsumption < 0f)
        {
            return 0m;
        }

        return powerCompProps.PowerConsumption.ToDecimal(0);
    }
}
