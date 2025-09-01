using CombatExtended;

namespace Stats.Compat.CE;

public sealed class Weapon_MagazineCapacityColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Weapon_MagazineCapacityColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var thingDef = thing.Def.building?.turretGunDef ?? thing.Def;

        return thingDef.GetStatValuePerceived(CE_StatDefOf.MagazineCapacity).ToDecimal(0);
    }
}
