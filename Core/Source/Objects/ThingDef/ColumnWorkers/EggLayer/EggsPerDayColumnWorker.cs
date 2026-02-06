using RimWorld;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.EggLayer;

public sealed class EggsPerDayColumnWorker : NumberColumnWorker<VirtualThing>
{
    public EggsPerDayColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0/d")
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        var eggLayerCompProps = thing.Def.GetCompProperties<CompProperties_EggLayer>();

        if (eggLayerCompProps is { eggLayIntervalDays: > 0f })
        {
            return (eggLayerCompProps.eggCountRange.Average / eggLayerCompProps.eggLayIntervalDays).ToDecimal(1);
        }

        return 0m;
    }
}
