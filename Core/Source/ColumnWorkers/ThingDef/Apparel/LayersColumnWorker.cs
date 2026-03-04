using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.Apparel;

public sealed class LayersColumnWorker(ColumnDef columnDef) : StaticColumnWorker<DefBasedObject,>
{
    public override ColumnDef Def => columnDef;

    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        ApparelProperties? apparelProps = thingDef.apparel;

        if (apparelProps != null)
        {
            return new DefSetTableCell.Constant(apparelProps.layers);
        }

        return DefSetTableCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<ApparelLayerDef> layerDefs = ((IRefRecordsProvider)tableWorker).Records
            .SelectMany(thingDef => thingDef.apparel?.layers)
            .Distinct();

        return DefSetTableCell.GetDescriptor(columnDef, layerDefs);
    }
}
