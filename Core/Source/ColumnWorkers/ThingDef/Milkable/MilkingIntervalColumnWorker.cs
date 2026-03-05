using RimWorld;
using Stats.TableCells;

namespace Stats.ColumnWorkers.ThingDef.Milkable;

public sealed class MilkingIntervalColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            CompProperties_Milkable? milkableCompProps = thingDef.GetCompProperties<CompProperties_Milkable>();

            if (milkableCompProps != null)
            {
                return new NumberTableCell(milkableCompProps.milkIntervalDays, "0 d");
            }
        }

        return default;
    }
}
