using RimWorld;
using Stats.Objects.ThingDef;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.Apparel;

public sealed class Reloadable_MaxChargesCountColumnWorker : NumberColumnWorker<VirtualThing>
{
    public Reloadable_MaxChargesCountColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        var reloadableCompProperties = thing.Def.GetCompProperties<CompProperties_ApparelReloadable>();

        if (reloadableCompProperties == null)
        {
            return 0m;
        }

        return reloadableCompProperties.maxCharges;
    }
}
