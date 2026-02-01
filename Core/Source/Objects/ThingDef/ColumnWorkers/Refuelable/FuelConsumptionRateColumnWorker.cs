using RimWorld;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;

namespace Stats.Objects.ThingDef.ColumnWorkers.Refuelable;

public sealed class FuelConsumptionRateColumnWorker(ColumnDef columnDef) :
    IColumnWorker<Verse.ThingDef>,
    IColumnWorker<VirtualThing>,
    IColumnWorker<Verse.Thing>
{
    public Cell GetCell(Verse.Thing thing) => GetCell(thing.def);
    public Cell GetCell(VirtualThing thing) => GetCell(thing.Def);
    public Cell GetCell(Verse.ThingDef thingDef)
    {
        CompProperties_Refuelable? refuelableCompProps = thingDef.GetCompProperties<CompProperties_Refuelable>();

        if (refuelableCompProps != null)
        {
            // TODO: Difficulty scaling.
            decimal cellValue = refuelableCompProps.fuelConsumptionRate.ToDecimal(1);

            return new NumberCell(cellValue, "0.0/d");
        }

        return NumberCell.Empty;
    }
    public CellDescriptor GetCellDescriptor() => NumberCell.GetDescriptor(columnDef);
}
