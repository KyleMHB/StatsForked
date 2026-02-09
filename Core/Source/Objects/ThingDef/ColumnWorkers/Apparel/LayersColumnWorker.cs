using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Objects.ThingDef.TableWorkers;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.Apparel;

public sealed class LayersColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        ApparelProperties? apparelProps = thingDef.apparel;

        if (apparelProps != null)
        {
            return new DefSetCell(apparelProps.layers);
        }

        return DefSetCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<ApparelLayerDef> layerDefs = ((IRefRecordsProvider)tableWorker).Records
            .SelectMany(thingDef => thingDef.apparel?.layers)
            .Distinct();

        return DefSetCell.GetDescriptor(columnDef, layerDefs);
    }
}
