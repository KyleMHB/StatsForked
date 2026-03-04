using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Apparel.Reloadable;

public sealed class MaxChargesCountColumnWorker(ColumnDef columnDef) : StaticColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        CompProperties_ApparelReloadable? reloadableCompProperties = thingDef.GetCompProperties<CompProperties_ApparelReloadable>();

        if (reloadableCompProperties != null)
        {
            return new NumberTableCell.Constant(reloadableCompProperties.maxCharges);
        }

        return default;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberTableCell.GetDescriptor(columnDef);
}
