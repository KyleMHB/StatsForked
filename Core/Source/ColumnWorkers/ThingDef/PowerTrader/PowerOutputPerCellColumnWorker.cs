using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.PowerTrader;

public sealed class PowerOutputPerCellColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        CompProperties_Power? powerCompProps = thingDef.GetCompProperties<CompProperties_Power>();

        if (powerCompProps != null)
        {
            decimal cellValue = powerCompProps.PowerConsumption.ToDecimal(0) * -1m / thingDef.size.Area;

            return new NumberCell.Constant(cellValue, "0 W/c");
        }

        return NumberCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
