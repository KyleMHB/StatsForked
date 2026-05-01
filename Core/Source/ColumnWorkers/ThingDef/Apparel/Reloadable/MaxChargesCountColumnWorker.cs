using RimWorld;
using Stats.ColumnWorkers.Cells;

namespace Stats.ColumnWorkers.ThingDef.Apparel.Reloadable;

public sealed class MaxChargesCountColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            CompProperties_ApparelReloadable? reloadableCompProperties = thingDef.GetCompProperties<CompProperties_ApparelReloadable>();

            if (reloadableCompProperties != null)
            {
                return new NumberCell(reloadableCompProperties.maxCharges);
            }
        }

        return default;
    }
}
