using RimWorld;
using Stats.TableCells;

namespace Stats.ColumnWorkers.ThingDef.Refuelable;

public sealed class DaysPerRefuelColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            CompProperties_Refuelable? refuelableCompProps = thingDef.GetCompProperties<CompProperties_Refuelable>();

            if (refuelableCompProps is { fuelConsumptionRate: not 0f })
            {
                decimal cellValue = (refuelableCompProps.fuelCapacity / refuelableCompProps.fuelConsumptionRate).ToDecimal(1);

                return new NumberTableCell(cellValue, "0.0 d");
            }
        }

        return default;
    }
}
