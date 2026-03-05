using RimWorld;
using Stats.TableCells;

namespace Stats.ColumnWorkers.ThingDef.Milkable;

public sealed class MilkPerDayColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            CompProperties_Milkable? milkableCompProps = thingDef.GetCompProperties<CompProperties_Milkable>();

            if (milkableCompProps is { milkIntervalDays: > 0 })
            {
                decimal cellValue = ((float)milkableCompProps.milkAmount / milkableCompProps.milkIntervalDays).ToDecimal(1);

                return new NumberTableCell(cellValue, "0.0/d");
            }
        }

        return default;
    }
}
