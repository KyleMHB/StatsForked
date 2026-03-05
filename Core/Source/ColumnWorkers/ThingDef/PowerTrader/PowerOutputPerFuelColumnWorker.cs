using RimWorld;
using Stats.TableCells;

namespace Stats.ColumnWorkers.ThingDef.PowerTrader;

public sealed class PowerOutputPerFuelColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            CompProperties_Power? powerCompProps = thingDef.GetCompProperties<CompProperties_Power>();
            CompProperties_Refuelable? refuelableCompProps = thingDef.GetCompProperties<CompProperties_Refuelable>();

            if (powerCompProps != null && refuelableCompProps is { fuelConsumptionRate: not 0f })
            {
                float powerOutput = powerCompProps.PowerConsumption * -1f;
                float fuelConsumptionRate = refuelableCompProps.fuelConsumptionRate;
                decimal cellValue = (powerOutput / fuelConsumptionRate).ToDecimal(0);

                return new NumberTableCell(cellValue, "0 W/u");
            }
        }

        return default;
    }
}
