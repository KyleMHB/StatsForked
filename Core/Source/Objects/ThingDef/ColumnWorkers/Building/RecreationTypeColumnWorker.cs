using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Objects.ThingDef.TableWorkers;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;

namespace Stats.Objects.ThingDef.ColumnWorkers.Building;

public sealed class RecreationTypeColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        JoyKindDef? joyKind = thingDef.building?.joyKind;

        if (joyKind != null)
        {
            return new DefCell(joyKind);
        }

        return DefCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<JoyKindDef?> joyKindDefs = ((IRefRecordsProvider)tableWorker).Records
            .Select(thingDef => thingDef.building?.joyKind)
            .Distinct();

        return DefCell.GetDescriptor(columnDef, joyKindDefs);
    }
}
