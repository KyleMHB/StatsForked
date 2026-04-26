using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.ColumnWorkers;
using Stats.ColumnWorkers.Cells;
using Stats.TableWorkers;
using Verse;

namespace Stats.Compat.Biotech;

public sealed class Mech_RechargerNeededColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker<DefBasedObject, ThingDefCell>
{
    public override ColumnDef Def => columnDef;

    protected override ThingDefCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is ThingDef thingDef)
        {
            ThingDef? rechargerThingDef = MechanitorUtility.RechargerForMech(thingDef);
            if (rechargerThingDef != null)
            {
                return new ThingDefCell(rechargerThingDef);
            }
        }

        return default;
    }

    protected override IEnumerable<ThingDef?> GetValueFieldFilterOptions(TableWorker tableWorker)
    {
        return ((IRefRecordsProvider<ThingDef>)tableWorker).Records
            .Select(MechanitorUtility.RechargerForMech)
            .Distinct();
    }
}
