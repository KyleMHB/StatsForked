using RimWorld;
using Stats.Objects.ThingDef;

namespace Stats.Objects.ThingDef.ColumnWorkers.Apparel;

public sealed class Reloadable_IsDestroyedOnEmptyColumnWorker : BooleanColumnWorker<VirtualThing>
{
    public Reloadable_IsDestroyedOnEmptyColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetCellValue(VirtualThing thing)
    {
        var reloadableCompProperties = thing.Def.GetCompProperties<CompProperties_ApparelReloadable>();

        if (reloadableCompProperties == null)
        {
            return false;
        }

        return reloadableCompProperties.destroyOnEmpty;
    }
}
