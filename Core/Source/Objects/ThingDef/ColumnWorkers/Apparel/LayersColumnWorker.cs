using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Objects.ThingDef;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.Apparel;

public sealed class LayersColumnWorker : DefSetColumnWorker<VirtualThing, ApparelLayerDef>
{
    public LayersColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override HashSet<ApparelLayerDef> GetValue(VirtualThing thing)
    {
        return GetApparelLayerDefSet(thing.Def);
    }
    private static readonly Func<ThingDef, HashSet<ApparelLayerDef>> GetApparelLayerDefSet =
    FunctionExtensions.Memoized((ThingDef thingDef) =>
    {
        return thingDef.apparel?.layers.ToHashSet() ?? [];
    });
}
