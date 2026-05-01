using CombatExtended;
using Stats.ColumnWorkers;
using Stats.ColumnWorkers.Cells;
using Stats.Utils.Extensions;

namespace Stats.Compat.CE;

public sealed class Weapon_MagazineCapacityColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is not Verse.ThingDef thingDef)
        {
            return default;
        }

        Verse.ThingDef gunDef = thingDef.building?.turretGunDef ?? thingDef;
        decimal value = gunDef.GetStatValuePerceived(CE_StatDefOf.MagazineCapacity, quality: @object.Quality).ToDecimal(0);
        return value == 0m ? default : new NumberCell(value, "0");
    }
}
