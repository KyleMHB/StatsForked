namespace Stats;

public sealed class Weapon_ProjectileBuildingDamageFactorImpassableColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Weapon_ProjectileBuildingDamageFactorImpassableColumnWorker(ColumnDef columnDef) : base(columnDef, formatString: "0\\%")
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var thingDef = thing.Def.building?.turretGunDef ?? thing.Def;
        var verb = thingDef.Verbs.Primary();
        var damageDef = verb?.defaultProjectile?.projectile?.damageDef;

        if (damageDef != null)
        {
            return (100f * damageDef.buildingDamageFactorImpassable).ToDecimal(0);
        }

        return 0m;
    }
}
