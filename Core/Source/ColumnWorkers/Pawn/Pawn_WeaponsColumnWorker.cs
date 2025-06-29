using System.Collections.Generic;
using Verse;

namespace Stats;

public sealed class Pawn_WeaponsColumnWorker : ThingDefSetColumnWorker<ThingAlike, ThingDef>
{
    public Pawn_WeaponsColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override HashSet<ThingDef> GetValue(ThingAlike thing)
    {
        return thing.Def.GetPossibleWeapons() ?? [];
    }
}
