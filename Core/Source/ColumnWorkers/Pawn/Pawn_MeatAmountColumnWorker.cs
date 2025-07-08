using RimWorld;

namespace Stats;

public sealed class Pawn_MeatAmountColumnWorker : ThingDefCountColumnWorker<ThingAlike>
{
    public Pawn_MeatAmountColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override ThingDefCount? GetValue(ThingAlike thing)
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

        return null;
    }
}
