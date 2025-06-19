using RimWorld;

namespace Stats;

public sealed class Apparel_MaxChargesCountColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Apparel_MaxChargesCountColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var reloadableCompProperties = thing.Def.GetCompProperties<CompProperties_ApparelReloadable>();

        if (reloadableCompProperties == null)
        {
            return 0m;
        }

        return reloadableCompProperties.maxCharges;
    }
}
