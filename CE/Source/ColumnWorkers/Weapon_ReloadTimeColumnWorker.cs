using CombatExtended;
using RimWorld;
using Stats.ColumnWorkers;
using Stats.ColumnWorkers.Cells;
using Stats.Utils.Extensions;
using Verse;

namespace Stats.Compat.CE;

public sealed class Weapon_ReloadTimeColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is not Verse.ThingDef thingDef)
        {
            return default;
        }

        Verse.ThingDef gunDef = thingDef.building?.turretGunDef ?? thingDef;
        StatRequest statRequest = StatRequest.For(gunDef, null, @object.Quality);
        if (CE_StatDefOf.MagazineCapacity.Worker.ShouldShowFor(statRequest) == false)
        {
            return default;
        }

        decimal value = CE_StatDefOf.ReloadTime.Worker.GetValue(statRequest).ToDecimal(2);
        return value == 0m ? default : new NumberCell(value, "0.00 " + "LetterSecond".Translate());
    }
}
