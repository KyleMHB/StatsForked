using Verse;

namespace Stats;

public sealed class Weapon_RPMColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public Weapon_RPMColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb is { Ranged: true, showBurstShotStats: true, burstShotCount: > 1 })
        {
            return (60f / verb.ticksBetweenBurstShots.TicksToSeconds()).ToString("0.## rpm");
        }

        return "";
    }
}
