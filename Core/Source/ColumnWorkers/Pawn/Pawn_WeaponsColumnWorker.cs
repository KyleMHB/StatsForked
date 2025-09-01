using System.Collections.Generic;
using Verse;

namespace Stats;

public sealed class Pawn_WeaponsColumnWorker : ThingDefSetColumnWorker<AbstractThing, ThingDef>
{
    public Pawn_WeaponsColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override HashSet<ThingDef> GetValue(AbstractThing thing)
    {
        return thing.Def.GetPossibleWeapons() ?? [];
    }
}
