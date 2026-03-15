using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableWorkers;
using Stats.ColumnWorkers.Cells;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.ThingDef.Pawn;

public sealed class LeatherAmountColumnWorker(ColumnDef columnDef) : ThingDefCountColumnWorker<DefBasedObject, ThingDefCountCell>
{
    public override ColumnDef Def => columnDef;

    protected override ThingDefCountCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            Verse.ThingDef? leatherDef = thingDef.race?.leatherDef;

            if (leatherDef != null)
            {
                float leatherAmount = thingDef.GetStatValuePerceived(StatDefOf.LeatherAmount);

                if (leatherAmount > 0f)
                {
                    decimal cellValue = leatherAmount.ToDecimal(0);

                    return new ThingDefCountCell(leatherDef, cellValue);
                }
            }
        }

        return default;
    }

    protected override IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(TableWorker tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.race?.leatherDef)
            .Distinct();
    }
}
