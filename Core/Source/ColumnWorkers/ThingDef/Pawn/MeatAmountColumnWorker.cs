using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Pawn;

public sealed class MeatAmountColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        Verse.ThingDef? meatDef = thingDef.race?.meatDef;

        if (meatDef != null)
        {
            float meatAmount = thingDef.GetStatValuePerceived(StatDefOf.MeatAmount);

            if (meatAmount > 0f)
            {
                ThingDefCount cellValue = new(meatDef, meatAmount.ToDecimal(0));

                return new ThingDefCountCell(cellValue);
            }
        }

        return ThingDefCountTableCell.Empty;
    }

    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<Verse.ThingDef?> meatDefs = ((IRefRecordsProvider)tableWorker).Records
            .Select(thingDef => thingDef.race?.meatDef)
            .Distinct();

        return ThingDefCountTableCell.GetDescriptor(meatDefs);
    }
}
