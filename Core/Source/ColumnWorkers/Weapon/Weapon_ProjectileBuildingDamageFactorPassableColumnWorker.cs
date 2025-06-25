using Verse;

namespace Stats;

public sealed class Weapon_ProjectileBuildingDamageFactorPassableColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public Weapon_ProjectileBuildingDamageFactorPassableColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var thingDef = thing.Def.building?.turretGunDef ?? thing.Def;
        var verb = thingDef.Verbs.Primary();
        var damageDef = verb?.defaultProjectile?.projectile?.damageDef;

        if (damageDef != null && damageDef.buildingDamageFactorPassable != 1f)
        {
            return damageDef.buildingDamageFactorPassable.ToStringPercent();
        }

        return "";
    }
}
