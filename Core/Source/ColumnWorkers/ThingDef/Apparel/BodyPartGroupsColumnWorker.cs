using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.Apparel;

// In the game, this property is actually displayed as a list of all of the
// individual body parts that an apprel is covering. The resulting list may be
// huge. Displaying it in a single row will be a bad UX.
//
// Luckily, it looks like in a definition it is allowed to only list the whole
// groups of body parts. The resulting list is of course significantly smaller
// and can be safely displayed in a single row/column.
public sealed class BodyPartGroupsColumnWorker(ColumnDef columnDef) : StaticColumnWorker<DefBasedObject,>
{
    public override ColumnDef Def => columnDef;

    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        ApparelProperties? apparelProps = thingDef.apparel;

        if (apparelProps != null)
        {
            return new DefSetTableCell.Constant(apparelProps.bodyPartGroups);
        }

        return DefSetTableCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<BodyPartGroupDef> bodyPartGroupDefs = ((IRefRecordsProvider)tableWorker).Records
            .SelectMany(thingDef => thingDef.apparel?.bodyPartGroups)
            .Distinct();

        return DefSetTableCell.GetDescriptor(columnDef, bodyPartGroupDefs);
    }
}
