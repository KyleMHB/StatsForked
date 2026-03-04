using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Refuelable;

public sealed class FuelConsumptionRateColumnWorker(ColumnDef columnDef) : StaticColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            CompProperties_Refuelable? refuelableCompProps = thingDef.GetCompProperties<CompProperties_Refuelable>();

            if (refuelableCompProps != null)
            {
                // TODO: Difficulty scaling.
                decimal cellValue = refuelableCompProps.fuelConsumptionRate.ToDecimal(1);

                return new NumberTableCell(cellValue, "0.0/d");
            }
        }

        return default;
    }

    //public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberTableCell.GetDescriptor(columnDef);
}
