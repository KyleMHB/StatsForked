using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Objects.ThingDef.TableWorkers;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.Apparel;

// In the game, this property is actually displayed as a list of all of the
// individual body parts that an apprel is covering. The resulting list may be
// huge. Displaying it in a single row will be a bad UX.
//
// Luckily, it looks like in a definition it is allowed to only list the whole
// groups of body parts. The resulting list is of course significantly smaller
// and can be safely displayed in a single row/column.
public sealed class BodyPartGroupsColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell GetCell(Verse.ThingDef thingDef)
    {
        ApparelProperties? apparelProps = thingDef.apparel;

        if (apparelProps != null)
        {
            return new DefSetCell(apparelProps.bodyPartGroups);
        }

        return DefSetCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<BodyPartGroupDef> bodyPartGroupDefs = ((IRefRecordsProvider)tableWorker).Records
            .SelectMany(thingDef => thingDef.apparel?.bodyPartGroups)
            .Distinct();

        return DefSetCell.GetDescriptor(columnDef, bodyPartGroupDefs);
    }
}
