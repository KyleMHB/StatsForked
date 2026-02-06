using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Objects.ThingDef.TableWorkers;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;

namespace Stats.Objects.ThingDef.ColumnWorkers.Milkable;

public sealed class MilkAmountColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell GetCell(Verse.ThingDef thingDef)
    {
        var milkableCompProps = thingDef.GetCompProperties<CompProperties_Milkable>();

        if (milkableCompProps != null)
        {
            ThingDefCount cellValue = new(milkableCompProps.milkDef, milkableCompProps.milkAmount);

            return new ThingDefCountCell(cellValue);
        }

        return ThingDefCountCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<Verse.ThingDef?> milkDefs = ((IRefRecordsProvider)tableWorker).Records
            .Select(thingDef => thingDef.GetCompProperties<CompProperties_Milkable>()?.milkDef)
            .Distinct();

        return ThingDefCountCell.GetDescriptor(milkDefs);
    }
}
