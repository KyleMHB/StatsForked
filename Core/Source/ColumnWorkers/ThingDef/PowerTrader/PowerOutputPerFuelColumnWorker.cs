using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.PowerTrader;

public sealed class PowerOutputPerFuelColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        CompProperties_Power? powerCompProps = thingDef.GetCompProperties<CompProperties_Power>();
        CompProperties_Refuelable? refuelableCompProps = thingDef.GetCompProperties<CompProperties_Refuelable>();

        if (powerCompProps != null && refuelableCompProps is { fuelConsumptionRate: not 0f })
        {
            float powerOutput = powerCompProps.PowerConsumption * -1f;
            float fuelConsumptionRate = refuelableCompProps.fuelConsumptionRate;
            decimal cellValue = (powerOutput / fuelConsumptionRate).ToDecimal(0);

            return new NumberCell.Constant(cellValue, "0 W/u");
        }

        return NumberCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
