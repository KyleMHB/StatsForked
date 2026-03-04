using System.Collections.Generic;
using System.Linq;
using Stats;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Pawn;

public sealed class WeaponsColumnWorker(ColumnDef columnDef) : StaticColumnWorker<DefBasedObject,>
{
    public override ColumnDef Def => columnDef;

    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
        }
        HashSet<Verse.ThingDef>? weapons = thingDef.GetPossibleWeapons();

        if (weapons != null)
        {
            return new ThingDefSetTableCell(weapons);
        }

        return ThingDefSetTableCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<Verse.ThingDef> weapons = ((IRefRecordsProvider)tableWorker).Records
            .SelectMany(thingDef => thingDef.GetPossibleWeapons() ?? [])
            .Distinct();

        return ThingDefSetTableCell.GetDescriptor(columnDef, weapons);
    }
}
