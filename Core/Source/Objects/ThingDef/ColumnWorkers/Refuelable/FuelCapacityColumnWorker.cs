using RimWorld;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;

namespace Stats.Objects.ThingDef.ColumnWorkers.Refuelable;

public sealed class FuelCapacityColumnWorker(ThingDefCountColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell GetCell(Verse.ThingDef thingDef)
    {
        CompProperties_Refuelable? refuelableCompProps = thingDef.GetCompProperties<CompProperties_Refuelable>();

        if (refuelableCompProps != null)
        {
            Verse.ThingDef? fuelType = refuelableCompProps.fuelFilter?.AnyAllowedDef;

            if (fuelType != null)
            {
                decimal fuelCapacity = refuelableCompProps.fuelCapacity.ToDecimal(0);
                ThingDefCount cellValue = new(fuelType, fuelCapacity);

                return new ThingDefCountCell(cellValue);
            }
        }

        return ThingDefCountCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor() => ThingDefCountCell.GetDescriptor(columnDef);
}
