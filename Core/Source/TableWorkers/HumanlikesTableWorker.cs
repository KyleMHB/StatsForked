using Verse;

namespace Stats;

public sealed class HumanlikesTableWorker : ThingsTableWorker<Pawn>
{
    public HumanlikesTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThing(Pawn pawn)
    {
        return pawn.RaceProps.Humanlike;
    }
}
