using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Extensions;
using Stats.TableWorkers;
using Stats.ColumnWorkers.Cells;

namespace Stats.ColumnWorkers.ThingDef.Pawn;

public sealed class MeatAmountColumnWorker(ColumnDef columnDef) : ThingDefCountColumnWorker<DefBasedObject, ThingDefCountTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override ThingDefCountTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            Verse.ThingDef? meatDef = thingDef.race?.meatDef;

            if (meatDef != null)
            {
                float meatAmount = thingDef.GetStatValuePerceived(StatDefOf.MeatAmount);

                if (meatAmount > 0f)
                {
                    decimal cellValue = meatAmount.ToDecimal(0);

                    return new ThingDefCountTableCell(meatDef, cellValue);
                }
            }
        }

        return default;
    }

    protected override IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(TableWorker tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.race?.meatDef)
            .Distinct();
    }
}
