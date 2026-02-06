using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Objects.ThingDef.TableWorkers;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;

namespace Stats.Objects.ThingDef.ColumnWorkers.Pawn;

public sealed class LeatherAmountColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell GetCell(Verse.ThingDef thingDef)
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

        return ThingDefCountCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<Verse.ThingDef?> leatherDefs = ((IRefRecordsProvider)tableWorker).Records
            .Select(thingDef => thingDef.race?.leatherDef)
            .Distinct();

        return ThingDefCountCell.GetDescriptor(leatherDefs);
    }
}
