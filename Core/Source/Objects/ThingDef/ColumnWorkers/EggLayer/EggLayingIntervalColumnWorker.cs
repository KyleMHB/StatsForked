using RimWorld;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.EggLayer;

public sealed class EggLayingIntervalColumnWorker : NumberColumnWorker<VirtualThing>
{
    public EggLayingIntervalColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0 d")
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        var eggLayerCompProps = thing.Def.GetCompProperties<CompProperties_EggLayer>();

        if (eggLayerCompProps != null)
        {
            return eggLayerCompProps.eggLayIntervalDays.ToDecimal(1);
        }

        return 0m;
    }
}
