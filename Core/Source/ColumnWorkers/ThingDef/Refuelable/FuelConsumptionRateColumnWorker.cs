using RimWorld;
using Stats.Extensions;
using Stats.TableCells;

namespace Stats.ColumnWorkers.ThingDef.Refuelable;

public sealed class FuelConsumptionRateColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberTableCell>
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
}
