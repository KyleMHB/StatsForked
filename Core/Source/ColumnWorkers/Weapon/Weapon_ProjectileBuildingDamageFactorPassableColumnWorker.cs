namespace Stats;

public sealed class Weapon_ProjectileBuildingDamageFactorPassableColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Weapon_ProjectileBuildingDamageFactorPassableColumnWorker(ColumnDef columnDef) : base(columnDef, formatString: "0\\%")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var thingDef = thing.Def.building?.turretGunDef ?? thing.Def;
        var verb = thingDef.Verbs.Primary();
        var damageDef = verb?.defaultProjectile?.projectile?.damageDef;

        if (damageDef != null)
        {
            return (100f * damageDef.buildingDamageFactorPassable).ToDecimal(0);
        }

        return 0m;
    }
}
