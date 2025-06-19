using Verse;

namespace Stats;

public sealed class Pawn_MeatTypeColumnWorker : ThingDefColumnWorker<ThingAlike, ThingDef?>
{
    public Pawn_MeatTypeColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override ThingDef? GetValue(ThingAlike thing)
    {
        return thing.Def.race?.meatDef;
    }
}
