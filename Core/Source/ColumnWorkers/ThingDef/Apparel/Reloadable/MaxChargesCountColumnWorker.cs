using RimWorld;
using Stats.TableCells;

namespace Stats.ColumnWorkers.ThingDef.Apparel.Reloadable;

public sealed class MaxChargesCountColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            CompProperties_ApparelReloadable? reloadableCompProperties = thingDef.GetCompProperties<CompProperties_ApparelReloadable>();

            if (reloadableCompProperties != null)
            {
                return new NumberTableCell(reloadableCompProperties.maxCharges);
            }
        }

        return default;
    }
}
