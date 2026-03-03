using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Refuelable;

public sealed class DaysPerRefuelColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        CompProperties_Refuelable? refuelableCompProps = thingDef.GetCompProperties<CompProperties_Refuelable>();

        if (refuelableCompProps is { fuelConsumptionRate: not 0f })
        {
            decimal cellValue = (refuelableCompProps.fuelCapacity / refuelableCompProps.fuelConsumptionRate).ToDecimal(1);

            return new NumberCell.Constant(cellValue, "0.0 d");
        }

        return NumberCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
