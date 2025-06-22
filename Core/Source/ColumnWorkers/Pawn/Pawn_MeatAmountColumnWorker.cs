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
            var statRequest = StatRequest.For(thing.Def, null);

            if (StatDefOf.MeatAmount.Worker.ShouldShowFor(statRequest))
            {
                var meatAmount = StatDefOf.MeatAmount.Worker.GetValue(statRequest);

                return new(meatDef, meatAmount.ToDecimal(0));
            }
        }

        return null;
    }
}
