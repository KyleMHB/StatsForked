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
            var statRequest = StatRequest.For(thing.Def, null);

            if (StatDefOf.LeatherAmount.Worker.ShouldShowFor(statRequest))
            {
                var leatherAmount = StatDefOf.LeatherAmount.Worker.GetValue(statRequest);

                return new(leatherDef, leatherAmount.ToDecimal(0));
            }
        }

        return null;
    }
}
