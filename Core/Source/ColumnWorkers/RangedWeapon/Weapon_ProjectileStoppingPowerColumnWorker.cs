namespace Stats;

public sealed class Weapon_ProjectileStoppingPowerColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Weapon_ProjectileStoppingPowerColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0")
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var thingDef = thing.Def.building?.turretGunDef ?? thing.Def;
        var verb = thingDef.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        return defaultProj?.stoppingPower.ToDecimal(1) ?? 0m;
    }
}
