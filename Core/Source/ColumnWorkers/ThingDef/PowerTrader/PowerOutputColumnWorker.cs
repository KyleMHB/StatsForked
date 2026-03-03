using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.PowerTrader;

public sealed class PowerOutputColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        CompProperties_Power? powerCompProps = thingDef.GetCompProperties<CompProperties_Power>();

        if (powerCompProps is { PowerConsumption: > 0f })
        {
            decimal cellValue = powerCompProps.PowerConsumption.ToDecimal(0) * -1m;

            return new NumberCell.Constant(cellValue, "0 W");
        }

        return NumberCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
