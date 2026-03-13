using System.Collections.Generic;
using System.Linq;
using Stats.ColumnWorkers.Cells;
using Stats.TableWorkers;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.Animal;

public sealed class TrainabilityColumnWorker(ColumnDef columnDef) : DefColumnWorker<DefBasedObject, DefTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override DefTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            TrainabilityDef? trainability = thingDef.race?.trainability;

            if (trainability != null)
            {
                return new DefTableCell(trainability);
            }
        }

        return default;
    }

    protected override IEnumerable<Verse.Def?> GetValueFieldFilterOptions(TableWorker tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.race?.trainability)
            .Distinct();
    }
}
