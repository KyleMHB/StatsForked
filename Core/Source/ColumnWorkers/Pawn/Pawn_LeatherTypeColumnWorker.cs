using Verse;

namespace Stats;

public sealed class Pawn_LeatherTypeColumnWorker : ThingDefColumnWorker<ThingAlike, ThingDef?>
{
    public Pawn_LeatherTypeColumnWorker(ColumnDef columnDef) : base(columnDef, false)
    {
    }
    protected override ThingDef? GetValue(ThingAlike thing)
    {
        return thing.Def.race?.leatherDef;
    }
}
