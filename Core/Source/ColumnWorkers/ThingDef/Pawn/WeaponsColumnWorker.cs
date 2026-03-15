using System.Collections.Generic;
using System.Linq;
using Stats.TableWorkers;
using Stats.ColumnWorkers.Cells;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.ThingDef.Pawn;

public sealed class WeaponsColumnWorker(ColumnDef columnDef) : ThingDefSetColumnWorker<DefBasedObject, ThingDefSetCell>
{
    public override ColumnDef Def => columnDef;

    protected override ThingDefSetCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            HashSet<Verse.ThingDef>? weapons = thingDef.GetPossibleWeapons();

            if (weapons != null)
            {
                return new ThingDefSetCell(weapons);
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
