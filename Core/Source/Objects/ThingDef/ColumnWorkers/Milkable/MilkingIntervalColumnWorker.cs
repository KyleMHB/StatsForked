using RimWorld;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.Milkable;

public sealed class MilkingIntervalColumnWorker : NumberColumnWorker<VirtualThing>
{
    public MilkingIntervalColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0 d")
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        var milkableCompProps = thing.Def.GetCompProperties<CompProperties_Milkable>();

        if (milkableCompProps != null)
        {
            return milkableCompProps.milkIntervalDays;
        }

        return 0m;
    }
}
