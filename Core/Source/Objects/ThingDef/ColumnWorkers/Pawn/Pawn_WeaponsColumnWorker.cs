using System.Collections.Generic;
using Stats.Objects.ThingDef;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.Pawn;

public sealed class Pawn_WeaponsColumnWorker : ThingDefSetColumnWorker<VirtualThing, ThingDef>
{
    public Pawn_WeaponsColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override HashSet<ThingDef> GetValue(VirtualThing thing)
    {
        return thing.Def.GetPossibleWeapons() ?? [];
    }
}
