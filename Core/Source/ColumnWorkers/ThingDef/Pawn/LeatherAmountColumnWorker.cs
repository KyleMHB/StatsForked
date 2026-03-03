using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Pawn;

public sealed class LeatherAmountColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        Verse.ThingDef? leatherDef = thingDef.race?.leatherDef;

        if (leatherDef != null)
        {
            float leatherAmount = thingDef.GetStatValuePerceived(StatDefOf.LeatherAmount);

            if (leatherAmount > 0f)
            {
                ThingDefCount cellValue = new(leatherDef, leatherAmount.ToDecimal(0));

                return new ThingDefCountCell(cellValue);
            }
        }

        return ThingDefCountTableCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<Verse.ThingDef?> leatherDefs = ((IRefRecordsProvider)tableWorker).Records
            .Select(thingDef => thingDef.race?.leatherDef)
            .Distinct();

        return ThingDefCountTableCell.GetDescriptor(leatherDefs);
    }
}
