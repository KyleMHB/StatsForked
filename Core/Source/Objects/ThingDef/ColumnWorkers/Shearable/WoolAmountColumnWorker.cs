using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Objects.ThingDef.TableWorkers;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;

namespace Stats.Objects.ThingDef.ColumnWorkers.Shearable;

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

        return ThingDefCountCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<Verse.ThingDef?> woolDefs = ((IRefRecordsProvider)tableWorker).Records
            .Select(thingDef => thingDef.GetCompProperties<CompProperties_Shearable>()?.woolDef)
            .Distinct();

        return ThingDefCountCell.GetDescriptor(woolDefs);
    }
}
