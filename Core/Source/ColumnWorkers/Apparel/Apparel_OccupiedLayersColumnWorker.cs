using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Stats;

public sealed class Apparel_OccupiedLayersColumnWorker : DefSetColumnWorker<ThingAlike, ApparelLayerDef>
{
    public Apparel_OccupiedLayersColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override HashSet<ApparelLayerDef> GetValue(ThingAlike thing)
    {
        return thing.Def.apparel?.layers.ToHashSet() ?? [];
    }
}
