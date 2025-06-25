using Verse;

namespace Stats;

public sealed class Weapon_ProjectileBuildingDamageFactorImpassableColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public Weapon_ProjectileBuildingDamageFactorImpassableColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var thingDef = thing.Def.building?.turretGunDef ?? thing.Def;
        var verb = thingDef.Verbs.Primary();
        var damageDef = verb?.defaultProjectile?.projectile?.damageDef;

        if (damageDef != null && damageDef.buildingDamageFactorImpassable != 1f)
        {
            return damageDef.buildingDamageFactorImpassable.ToStringPercent();
        }

        return "";
    }
}
