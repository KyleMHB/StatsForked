using CombatExtended;

namespace Stats.Compat.CE;

public sealed class Weapon_MagazineCapacityColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Weapon_MagazineCapacityColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var thingDef = thing.Def.building?.turretGunDef ?? thing.Def;

        return thingDef.GetStatValuePerceived(CE_StatDefOf.MagazineCapacity).ToDecimal(0);
    }
}
