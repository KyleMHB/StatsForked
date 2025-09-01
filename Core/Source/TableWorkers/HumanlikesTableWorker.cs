using Verse;

namespace Stats;

public sealed class HumanlikesTableWorker : ThingTableWorker<Pawn>
{
    public HumanlikesTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThing(Pawn pawn)
    {
        return pawn.RaceProps.Humanlike;
    }
}
