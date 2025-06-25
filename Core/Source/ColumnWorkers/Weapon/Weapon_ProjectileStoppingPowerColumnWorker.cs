namespace Stats;

public sealed class Weapon_ProjectileStoppingPowerColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public Weapon_ProjectileStoppingPowerColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var thingDef = thing.Def.building?.turretGunDef ?? thing.Def;
        var verb = thingDef.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        return defaultProj?.stoppingPower.ToString("F1") ?? "";
    }
}
