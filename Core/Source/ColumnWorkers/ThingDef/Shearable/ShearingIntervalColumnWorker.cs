using RimWorld;
using Stats.ColumnWorkers.Cells;

namespace Stats.ColumnWorkers.ThingDef.Shearable;

public sealed class ShearingIntervalColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            CompProperties_Shearable? shearableCompProps = thingDef.GetCompProperties<CompProperties_Shearable>();

            if (shearableCompProps != null)
            {
                return new NumberCell(shearableCompProps.shearIntervalDays, "0 d");
            }
        }

        return default;
    }
}
