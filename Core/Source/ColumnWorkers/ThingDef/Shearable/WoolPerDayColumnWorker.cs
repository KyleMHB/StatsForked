using RimWorld;
using Stats.Extensions;
using Stats.ColumnWorkers.Cells;

namespace Stats.ColumnWorkers.ThingDef.Shearable;

public sealed class WoolPerDayColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            CompProperties_Shearable? shearableCompProps = thingDef.GetCompProperties<CompProperties_Shearable>();

            if (shearableCompProps is { shearIntervalDays: > 0 })
            {
                decimal cellValue = ((float)shearableCompProps.woolAmount / shearableCompProps.shearIntervalDays).ToDecimal(1);

                return new NumberTableCell(cellValue, "0.0/d");
            }
        }

        return default;
    }
}
