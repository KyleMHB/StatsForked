namespace Stats;

public sealed class Weapon_ProjectileStoppingPowerColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public Weapon_ProjectileStoppingPowerColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        return defaultProj?.stoppingPower.ToString("F1") ?? "";
    }
}
