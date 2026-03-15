using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.ThingDef.Refuelable;

public sealed class FuelConsumptionRateColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            CompProperties_Refuelable? refuelableCompProps = thingDef.GetCompProperties<CompProperties_Refuelable>();

            if (refuelableCompProps != null)
            {
                // TODO: Difficulty scaling.
                decimal cellValue = refuelableCompProps.fuelConsumptionRate.ToDecimal(1);

                return new NumberCell(cellValue, "0.0/d");
            }
        }

        return default;
    }
}
