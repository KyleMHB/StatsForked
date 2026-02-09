using RimWorld;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;

namespace Stats.Objects.ThingDef.ColumnWorkers.PowerTrader;

public sealed class PowerOutputPerCellColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        CompProperties_Power? powerCompProps = thingDef.GetCompProperties<CompProperties_Power>();

        if (powerCompProps != null)
        {
            decimal cellValue = powerCompProps.PowerConsumption.ToDecimal(0) * -1m / thingDef.size.Area;

            return new NumberCell(cellValue, "0 W/c");
        }

        return NumberCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
