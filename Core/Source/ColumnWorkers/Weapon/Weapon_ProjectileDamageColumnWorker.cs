namespace Stats;

public sealed class Weapon_ProjectileDamageColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Weapon_ProjectileDamageColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        if (defaultProj?.damageDef?.harmsHealth == true)
        {
            return defaultProj.GetDamageAmount(thing.Def, thing.StuffDef);
        }

        return 0m;
    }
}
