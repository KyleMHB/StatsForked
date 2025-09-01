namespace Stats;

public sealed class Weapon_ProjectileArmorPenetrationColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Weapon_ProjectileArmorPenetrationColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0\\%")
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var thingDef = thing.Def.building?.turretGunDef ?? thing.Def;
        var verb = thingDef.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        if (defaultProj?.damageDef is { harmsHealth: true, armorCategory: not null })
        {
            return (100f * defaultProj.GetArmorPenetration(null)).ToDecimal(0);
        }
        else if (defaultProj == null && verb?.beamDamageDef != null)
        {
            return (100f * verb.beamDamageDef.defaultArmorPenetration).ToDecimal(0);
        }

        return 0m;
    }
}
