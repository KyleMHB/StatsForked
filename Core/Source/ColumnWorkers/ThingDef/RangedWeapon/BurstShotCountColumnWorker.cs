using Stats.ColumnWorkers.Cells;
using Stats.Utils.Extensions;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.RangedWeapon;

public sealed class BurstShotCountColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            VerbProperties? verbProps = thingDef.TurretGunDefOrSelf().Verbs.Primary();

            if (verbProps is { Ranged: true, showBurstShotStats: true })
            {
                decimal cellValue = verbProps.burstShotCount;

                return new NumberCell(cellValue);
            }
        }

        return default;
    }
}
