using System.Collections.Generic;
using System.Linq;
using Stats.Extensions;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Pawn;

public sealed class WeaponsColumnWorker(ColumnDef columnDef) : ThingDefSetColumnWorker<DefBasedObject, ThingDefSetTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override ThingDefSetTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            HashSet<Verse.ThingDef>? weapons = thingDef.GetPossibleWeapons();

            if (weapons != null)
            {
                return new ThingDefSetTableCell(weapons);
            }
        }

        return default;
    }

    protected override IEnumerable<Verse.ThingDef?> GetValueFieldFilterOptions(TableWorker tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .SelectMany(thingDef => thingDef.GetPossibleWeapons() ?? [])
            .Distinct();
    }
}
