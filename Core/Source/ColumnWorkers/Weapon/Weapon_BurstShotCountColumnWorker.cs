namespace Stats;

public sealed class Weapon_BurstShotCountColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Weapon_BurstShotCountColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var thingDef = thing.Def.building?.turretGunDef ?? thing.Def;
        var verb = thingDef.Verbs.Primary();

        if (verb is { Ranged: true, showBurstShotStats: true })
        {
            return verb.burstShotCount;
        }

        return 0m;
    }
}
