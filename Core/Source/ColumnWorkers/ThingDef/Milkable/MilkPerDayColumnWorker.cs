using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Milkable;

public sealed class MilkPerDayColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        CompProperties_Milkable? milkableCompProps = thingDef.GetCompProperties<CompProperties_Milkable>();

        if (milkableCompProps is { milkIntervalDays: > 0 })
        {
            decimal cellValue = ((float)milkableCompProps.milkAmount / milkableCompProps.milkIntervalDays).ToDecimal(1);

            return new NumberCell.Constant(cellValue, "0.0/d");
        }

        return NumberCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
