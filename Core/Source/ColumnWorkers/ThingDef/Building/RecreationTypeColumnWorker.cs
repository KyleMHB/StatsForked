using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Building;

public sealed class RecreationTypeColumnWorker(ColumnDef columnDef) : StaticColumnWorker<DefBasedObject,>
{
    public override ColumnDef Def => columnDef;

    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
        }
        JoyKindDef? joyKind = thingDef.building?.joyKind;

        if (joyKind != null)
        {
            return new DefTableCell.Constant(joyKind);
        }

        return DefTableCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<JoyKindDef?> joyKindDefs = ((IRefRecordsProvider)tableWorker).Records
            .Select(thingDef => thingDef.building?.joyKind)
            .Distinct();

        return DefTableCell.GetDescriptor(columnDef, joyKindDefs);
    }
}
