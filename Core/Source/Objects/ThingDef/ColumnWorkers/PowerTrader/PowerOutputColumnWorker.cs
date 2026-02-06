using RimWorld;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;

namespace Stats.Objects.ThingDef.ColumnWorkers.PowerTrader;

public sealed class PowerOutputColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell GetCell(Verse.ThingDef thingDef)
    {
        CompProperties_Power? powerCompProps = thingDef.GetCompProperties<CompProperties_Power>();

        if (powerCompProps is { PowerConsumption: > 0f })
        {
            decimal cellValue = powerCompProps.PowerConsumption.ToDecimal(0) * -1m;

            return new NumberCell(cellValue, "0 W");
        }

        return NumberCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor() => NumberCell.GetDescriptor(columnDef);
}
