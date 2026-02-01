using RimWorld;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;

namespace Stats.Objects.ThingDef.ColumnWorkers.PowerTrader;

public sealed class PowerConsumptionColumnWorker(ColumnDef columnDef) :
    IColumnWorker<Verse.ThingDef>,
    IColumnWorker<VirtualThing>,
    IColumnWorker<Verse.Thing>
{
    public Cell GetCell(Verse.Thing thing) => GetCell(thing.def);
    public Cell GetCell(VirtualThing thing) => GetCell(thing.Def);
    public Cell GetCell(Verse.ThingDef thingDef)
    {
        CompProperties_Power? powerCompProps = thingDef.GetCompProperties<CompProperties_Power>();

        if (powerCompProps is { PowerConsumption: < 0f })
        {
            decimal cellValue = powerCompProps.PowerConsumption.ToDecimal(0);

            return new NumberCell(cellValue, "0 W");
        }

        return NumberCell.Empty;
    }
    public CellDescriptor GetCellDescriptor() => NumberCell.GetDescriptor(columnDef);
}
