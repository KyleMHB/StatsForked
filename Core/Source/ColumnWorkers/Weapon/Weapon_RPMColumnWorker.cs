using Verse;

namespace Stats;

public sealed class Weapon_RPMColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Weapon_RPMColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0 rpm")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var thingDef = thing.Def.building?.turretGunDef ?? thing.Def;
        var verb = thingDef.Verbs.Primary();

        // Reminder: This is not IRL RPM.
        if (verb is { Ranged: true, showBurstShotStats: true, burstShotCount: > 1 })
        {
            return (60f / verb.ticksBetweenBurstShots.TicksToSeconds()).ToDecimal(0);
        }

        return 0m;
    }
}
