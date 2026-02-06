using RimWorld;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.Milkable;

public sealed class MilkPerDayColumnWorker : NumberColumnWorker<VirtualThing>
{
    public MilkPerDayColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0/d")
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        var milkableCompProps = thing.Def.GetCompProperties<CompProperties_Milkable>();

        if (milkableCompProps is { milkIntervalDays: > 0 })
        {
            return ((float)milkableCompProps.milkAmount / milkableCompProps.milkIntervalDays).ToDecimal(1);
        }

        return 0m;
    }
}
