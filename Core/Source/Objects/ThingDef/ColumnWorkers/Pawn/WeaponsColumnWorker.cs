using System.Collections.Generic;
using System.Linq;
using Stats.Objects.ThingDef.TableWorkers;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;

namespace Stats.Objects.ThingDef.ColumnWorkers.Pawn;

public sealed class WeaponsColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        HashSet<Verse.ThingDef>? weapons = thingDef.GetPossibleWeapons();

        if (weapons != null)
        {
            return new ThingDefSetCell(weapons);
        }

        return ThingDefSetCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<Verse.ThingDef> weapons = ((IRefRecordsProvider)tableWorker).Records
            .SelectMany(thingDef => thingDef.GetPossibleWeapons() ?? [])
            .Distinct();

        return ThingDefSetCell.GetDescriptor(columnDef, weapons);
    }
}
