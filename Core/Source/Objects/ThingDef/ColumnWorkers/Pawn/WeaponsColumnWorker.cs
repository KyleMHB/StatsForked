using System.Collections.Generic;
using Stats.Objects.ThingDef;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.Pawn;

public sealed class WeaponsColumnWorker : ThingDefSetColumnWorker<VirtualThing, ThingDef>
{
    public WeaponsColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override HashSet<ThingDef> GetValue(VirtualThing thing)
    {
        return thing.Def.GetPossibleWeapons() ?? [];
    }
}
