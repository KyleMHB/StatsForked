using System;
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
        return GetApparelLayerDefSet(thing.Def);
    }
    private static readonly Func<ThingDef, HashSet<ApparelLayerDef>> GetApparelLayerDefSet =
    FunctionExtensions.Memoized((ThingDef thingDef) =>
    {
        return thingDef.apparel?.layers.ToHashSet() ?? [];
    });
}
