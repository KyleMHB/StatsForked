using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Milkable;

public sealed class MilkAmountColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        var milkableCompProps = thingDef.GetCompProperties<CompProperties_Milkable>();

        if (milkableCompProps != null)
        {
            ThingDefCount cellValue = new(milkableCompProps.milkDef, milkableCompProps.milkAmount);

            return new ThingDefCountCell(cellValue);
        }

        return ThingDefCountTableCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<Verse.ThingDef?> milkDefs = ((IRefRecordsProvider)tableWorker).Records
            .Select(thingDef => thingDef.GetCompProperties<CompProperties_Milkable>()?.milkDef)
            .Distinct();

        return ThingDefCountTableCell.GetDescriptor(milkDefs);
    }
}
