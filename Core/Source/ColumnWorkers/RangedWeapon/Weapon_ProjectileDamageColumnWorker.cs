namespace Stats;

public sealed class Weapon_ProjectileDamageColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Weapon_ProjectileDamageColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var thingDef = thing.Def.building?.turretGunDef ?? thing.Def;
        var verb = thingDef.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        if (defaultProj?.damageDef?.harmsHealth == true)
        {
            return defaultProj.GetDamageAmount(thingDef, null);
        }

        return 0m;
    }
}
