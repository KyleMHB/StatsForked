using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Apparel.Reloadable;

public sealed class MaxChargesCountColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        CompProperties_ApparelReloadable? reloadableCompProperties = thingDef.GetCompProperties<CompProperties_ApparelReloadable>();

        if (reloadableCompProperties != null)
        {
            return new NumberCell.Constant(reloadableCompProperties.maxCharges);
        }

        return NumberCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
