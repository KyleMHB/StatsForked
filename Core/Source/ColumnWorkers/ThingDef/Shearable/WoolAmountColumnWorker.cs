using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Shearable;

public sealed class WoolAmountColumnWorker(ColumnDef columnDef) : ThingDefCountColumnWorker<DefBasedObject, ThingDefCountTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override ThingDefCountTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            CompProperties_Shearable? shearableCompProps = thingDef.GetCompProperties<CompProperties_Shearable>();

            if (shearableCompProps != null)
            {
                return new ThingDefCountTableCell(shearableCompProps.woolDef, shearableCompProps.woolAmount);
            }
        }

        return default;
    }

    protected override IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(TableWorker tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.GetCompProperties<CompProperties_Shearable>()?.woolDef)
            .Distinct();
    }
}
