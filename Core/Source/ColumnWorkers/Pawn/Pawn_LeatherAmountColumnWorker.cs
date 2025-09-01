using RimWorld;
using Verse;

namespace Stats;

public sealed class Pawn_LeatherAmountColumnWorker : ThingDefCountColumnWorker<AbstractThing>
{
    public Pawn_LeatherAmountColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override (ThingDef? Def, decimal Count) GetValue(AbstractThing thing)
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

        return new();
    }
}
