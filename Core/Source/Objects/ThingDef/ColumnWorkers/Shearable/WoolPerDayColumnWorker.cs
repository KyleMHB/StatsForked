using RimWorld;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;

namespace Stats.Objects.ThingDef.ColumnWorkers.Shearable;

public sealed class WoolPerDayColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        CompProperties_Shearable? shearableCompProps = thingDef.GetCompProperties<CompProperties_Shearable>();

        if (shearableCompProps is { shearIntervalDays: > 0 })
        {
            decimal cellValue = ((float)shearableCompProps.woolAmount / shearableCompProps.shearIntervalDays).ToDecimal(1);

            return new NumberCell(cellValue, "0.0/d");
        }

        return NumberCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
