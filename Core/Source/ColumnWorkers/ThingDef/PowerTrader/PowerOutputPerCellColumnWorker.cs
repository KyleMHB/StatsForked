using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.PowerTrader;

public sealed class PowerOutputPerCellColumnWorker(ColumnDef columnDef) : StaticColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            CompProperties_Power? powerCompProps = thingDef.GetCompProperties<CompProperties_Power>();

            if (powerCompProps != null)
            {
                decimal cellValue = powerCompProps.PowerConsumption.ToDecimal(0) * -1m / thingDef.size.Area;

                return new NumberTableCell(cellValue, "0 W/c");
            }
        }

        return default;
    }
    //public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberTableCell.GetDescriptor(columnDef);
}
