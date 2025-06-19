using RimWorld;

namespace Stats;

public sealed class Apparel_IsDestroyedWhenAllChargesSpentColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public Apparel_IsDestroyedWhenAllChargesSpentColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(ThingAlike thing)
    {
        var reloadableCompProperties = thing.Def.GetCompProperties<CompProperties_ApparelReloadable>();

        if (reloadableCompProperties == null)
        {
            return false;
        }

        return reloadableCompProperties.destroyOnEmpty;
    }
}
