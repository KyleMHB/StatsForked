using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Building;

public sealed class RecreationTypeColumnWorker(ColumnDef columnDef) : DefColumnWorker<DefBasedObject, DefTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override DefTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            JoyKindDef? joyKind = thingDef.building?.joyKind;

            if (joyKind != null)
            {
                return new DefTableCell(joyKind);
            }
        }

        return default;
    }

    protected override IEnumerable<Verse.Def?> GetValueFieldFilterOptions(TableWorker tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.building?.joyKind)
            .Distinct();
    }
}
