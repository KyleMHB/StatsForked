using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Shearable;

public sealed class WoolAmountColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        CompProperties_Shearable? shearableCompProps = thingDef.GetCompProperties<CompProperties_Shearable>();

        if (shearableCompProps != null)
        {
            ThingDefCount cellValue = new(shearableCompProps.woolDef, shearableCompProps.woolAmount);

            return new ThingDefCountCell(cellValue);
        }

        return ThingDefCountTableCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<Verse.ThingDef?> woolDefs = ((IRefRecordsProvider)tableWorker).Records
            .Select(thingDef => thingDef.GetCompProperties<CompProperties_Shearable>()?.woolDef)
            .Distinct();

        return ThingDefCountTableCell.GetDescriptor(woolDefs);
    }
}
