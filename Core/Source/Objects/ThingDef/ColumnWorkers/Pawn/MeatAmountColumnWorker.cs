using RimWorld;
using Stats.ColumnWorkers_Legacy;
using Stats.Objects.ThingDef;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.Pawn;

public sealed class MeatAmountColumnWorker : ThingDefCountColumnWorker<VirtualThing>
{
    public MeatAmountColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override (ThingDef? Def, decimal Count) GetValue(VirtualThing thing)
    {
        var meatDef = thing.Def.race?.meatDef;

        if (meatDef != null)
        {
            var meatAmount = thing.Def.GetStatValuePerceived(StatDefOf.MeatAmount);

            if (meatAmount > 0f)
            {
                return new(meatDef, meatAmount.ToDecimal(0));
            }
        }

        return new();
    }
}
