using RimWorld;
using Stats.ColumnWorkers_Legacy;
using Stats.Objects.ThingDef;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.Pawn;

public sealed class Pawn_LeatherAmountColumnWorker : ThingDefCountColumnWorker<VirtualThing>
{
    public Pawn_LeatherAmountColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override (ThingDef? Def, decimal Count) GetValue(VirtualThing thing)
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
