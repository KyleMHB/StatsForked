using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Apparel;

public sealed class LayersColumnWorker(ColumnDef columnDef) : DefSetColumnWorker<DefBasedObject, DefSetCell>
{
    public override ColumnDef Def => columnDef;

    protected override DefSetCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            ApparelProperties? apparelProps = thingDef.apparel;

            if (apparelProps != null)
            {
                return new DefSetCell(apparelProps.layers);
            }
        }

        return default;
    }

    protected override IEnumerable<Verse.Def?> GetValueFieldFilterOptions(TableWorker tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .SelectMany(thingDef => thingDef.apparel?.layers)
            .Distinct();
    }
}
