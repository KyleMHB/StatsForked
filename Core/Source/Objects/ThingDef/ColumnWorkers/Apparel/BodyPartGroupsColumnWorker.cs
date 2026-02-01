using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Objects.ThingDef;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.Apparel;

// In the game, this property is actually displayed as a list of all of the
// individual body parts that an apprel is covering. The resulting list may be
// huge. Displaying it in a single row will be a bad UX.
//
// Luckily, it looks like in a definition it is allowed to only list the whole
// groups of body parts. The resulting list is of course significantly smaller
// and can be safely displayed in a single row/column.
public sealed class BodyPartGroupsColumnWorker : DefSetColumnWorker<VirtualThing, BodyPartGroupDef>
{
    public BodyPartGroupsColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override HashSet<BodyPartGroupDef> GetValue(VirtualThing thing)
    {
        return GetBodyPartGroupDefSet(thing.Def);
    }
    private static readonly Func<ThingDef, HashSet<BodyPartGroupDef>> GetBodyPartGroupDefSet =
    FunctionExtensions.Memoized((ThingDef thingDef) =>
    {
        return thingDef.apparel?.bodyPartGroups.ToHashSet() ?? [];
    });
}
