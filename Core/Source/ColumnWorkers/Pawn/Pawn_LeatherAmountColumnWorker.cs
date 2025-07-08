using RimWorld;

namespace Stats;

public sealed class Pawn_LeatherAmountColumnWorker : ThingDefCountColumnWorker<ThingAlike>
{
    public Pawn_LeatherAmountColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override ThingDefCount? GetValue(ThingAlike thing)
    {
        var leatherDef = thing.Def.race?.leatherDef;

        if (leatherDef != null)
        {
            var leatherAmount = thing.Def.GetStatValuePerceived(StatDefOf.LeatherAmount);

            if (leatherAmount > 0f)
            {
                return new(leatherDef, leatherAmount.ToDecimal(0));
            }
        }

        return null;
    }
}
