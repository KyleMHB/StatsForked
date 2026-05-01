using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Milkable;

public sealed class MilkAmountColumnWorker(ColumnDef columnDef) : ThingDefCountColumnWorker<DefBasedObject, ThingDefCountCell>
{
    public override ColumnDef Def => columnDef;

    protected override ThingDefCountCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            CompProperties_Milkable? milkableCompProps = thingDef.GetCompProperties<CompProperties_Milkable>();

            if (milkableCompProps != null)
            {
                return new ThingDefCountCell(milkableCompProps.milkDef, milkableCompProps.milkAmount);
            }
        }

        return default;
    }

    protected override IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(TableWorker tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.GetCompProperties<CompProperties_Milkable>()?.milkDef)
            .Distinct();
    }
}
