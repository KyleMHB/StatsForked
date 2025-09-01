using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Stats;

public sealed class Apparel_OccupiedLayersColumnWorker : DefSetColumnWorker<AbstractThing, ApparelLayerDef>
{
    public Apparel_OccupiedLayersColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override HashSet<ApparelLayerDef> GetValue(AbstractThing thing)
    {
        return GetApparelLayerDefSet(thing.Def);
    }
    private static readonly Func<ThingDef, HashSet<ApparelLayerDef>> GetApparelLayerDefSet =
    FunctionExtensions.Memoized((ThingDef thingDef) =>
    {
        return thingDef.apparel?.layers.ToHashSet() ?? [];
    });
}
