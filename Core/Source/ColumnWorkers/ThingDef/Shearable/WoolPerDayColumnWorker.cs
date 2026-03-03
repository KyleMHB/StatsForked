using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Shearable;

public sealed class WoolPerDayColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        CompProperties_Shearable? shearableCompProps = thingDef.GetCompProperties<CompProperties_Shearable>();

        if (shearableCompProps is { shearIntervalDays: > 0 })
        {
            decimal cellValue = ((float)shearableCompProps.woolAmount / shearableCompProps.shearIntervalDays).ToDecimal(1);

            return new NumberCell.Constant(cellValue, "0.0/d");
        }

        return NumberCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
