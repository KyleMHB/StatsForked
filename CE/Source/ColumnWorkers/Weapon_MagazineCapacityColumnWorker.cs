using CombatExtended;
using RimWorld;

namespace Stats.Compat.CE;

public sealed class Weapon_MagazineCapacityColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Weapon_MagazineCapacityColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var thingDef = thing.Def.building?.turretGunDef ?? thing.Def;
        var statRequest = StatRequest.For(thingDef, null);
        var worker = CE_StatDefOf.MagazineCapacity.Worker;

        if (worker.ShouldShowFor(statRequest))
        {
            return worker.GetValue(statRequest).ToDecimal(0);
        }

        return 0m;
    }
}
