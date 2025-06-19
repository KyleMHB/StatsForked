using Verse;

namespace Stats;

public sealed class Weapon_ProjectileArmorPenetrationColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public Weapon_ProjectileArmorPenetrationColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        if (defaultProj?.damageDef is { harmsHealth: true, armorCategory: not null })
        {
            return defaultProj.GetArmorPenetration(null).ToStringPercent();
        }
        else if (defaultProj == null && verb?.beamDamageDef != null)
        {
            return verb.beamDamageDef.defaultArmorPenetration.ToStringPercent();
        }

        return "";
    }
}
