using RimWorld;

namespace Stats;

public sealed class Apparel_MaxChargesCountColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Apparel_MaxChargesCountColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var reloadableCompProperties = thing.Def.GetCompProperties<CompProperties_ApparelReloadable>();

        if (reloadableCompProperties == null)
        {
            return 0m;
        }

        return reloadableCompProperties.maxCharges;
    }
}
