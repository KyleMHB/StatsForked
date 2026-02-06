using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Objects.ThingDef.TableWorkers;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;

namespace Stats.Objects.ThingDef.ColumnWorkers.Pawn;

public sealed class MeatAmountColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell GetCell(Verse.ThingDef thingDef)
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

        return ThingDefCountCell.Empty;
    }

    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<Verse.ThingDef?> meatDefs = ((IRefRecordsProvider)tableWorker).Records
            .Select(thingDef => thingDef.race?.meatDef)
            .Distinct();

        return ThingDefCountCell.GetDescriptor(meatDefs);
    }
}
