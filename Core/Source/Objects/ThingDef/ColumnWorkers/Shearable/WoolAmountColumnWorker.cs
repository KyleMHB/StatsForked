using RimWorld;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;

namespace Stats.Objects.ThingDef.ColumnWorkers.Shearable;

public sealed class WoolAmountColumnWorker(ThingDefCountColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell GetCell(Verse.ThingDef thingDef)
    {
        CompProperties_Shearable? shearableCompProps = thingDef.GetCompProperties<CompProperties_Shearable>();

        if (shearableCompProps != null)
        {
            ThingDefCount cellValue = new(shearableCompProps.woolDef, shearableCompProps.woolAmount);

            return new ThingDefCountCell(cellValue);
        }

        return ThingDefCountCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor() => ThingDefCountCell.GetDescriptor(columnDef);
}
