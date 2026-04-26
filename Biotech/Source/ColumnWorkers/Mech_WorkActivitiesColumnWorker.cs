using System.Collections.Generic;
using System.Linq;
using Stats.ColumnWorkers;
using Stats.ColumnWorkers.Cells;
using Stats.TableWorkers;
using Verse;

namespace Stats.Compat.Biotech;

public sealed class Mech_WorkActivitiesColumnWorker(ColumnDef columnDef) : DefSetColumnWorker<DefBasedObject, DefSetCell>
{
    public override ColumnDef Def => columnDef;

    protected override DefSetCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is ThingDef thingDef)
        {
            HashSet<Def> workTypes = thingDef.race?.mechEnabledWorkTypes?.Cast<Def>().ToHashSet() ?? [];
            if (workTypes.Count > 0)
            {
                return new DefSetCell(workTypes);
            }
        }

        return default;
    }

    protected override IEnumerable<Def?> GetValueFieldFilterOptions(TableWorker tableWorker)
    {
        return ((IRefRecordsProvider<ThingDef>)tableWorker).Records
            .SelectMany(thingDef => thingDef.race?.mechEnabledWorkTypes?.Cast<Def>() ?? [])
            .Distinct();
    }
}
