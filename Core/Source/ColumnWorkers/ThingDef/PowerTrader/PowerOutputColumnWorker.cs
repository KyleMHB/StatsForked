using RimWorld;
using Stats.TableCells;

namespace Stats.ColumnWorkers.ThingDef.PowerTrader;

public sealed class PowerOutputColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            CompProperties_Power? powerCompProps = thingDef.GetCompProperties<CompProperties_Power>();

            if (powerCompProps is { PowerConsumption: > 0f })
            {
                decimal cellValue = powerCompProps.PowerConsumption.ToDecimal(0) * -1m;

                return new NumberTableCell(cellValue, "0 W");
            }
        }

        return default;
    }
}
