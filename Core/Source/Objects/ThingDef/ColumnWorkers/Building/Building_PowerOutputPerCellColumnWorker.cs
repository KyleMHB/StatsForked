using Stats;
using Stats.Objects.ThingDef;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.Building;

public sealed class Building_PowerOutputPerCellColumnWorker : NumberColumnWorker<VirtualThing>
{
    public Building_PowerOutputPerCellColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0 W/c")
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        var powerCompProps = thing.Def.GetPowerCompProperties();

        if (powerCompProps == null)
        {
            return 0m;
        }

        return powerCompProps.PowerConsumption.ToDecimal(0) * -1m / thing.Def.size.Area;
    }
}
