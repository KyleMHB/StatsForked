namespace Stats;

public sealed class Weapon_BurstShotCountColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Weapon_BurstShotCountColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb is { Ranged: true, showBurstShotStats: true, burstShotCount: > 1 })
        {
            return verb.burstShotCount;
        }

        return 0m;
    }
}
