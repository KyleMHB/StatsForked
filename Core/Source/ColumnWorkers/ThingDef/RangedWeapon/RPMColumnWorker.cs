using Verse;
using Stats.ColumnWorkers.Cells;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.ThingDef.RangedWeapon;

public sealed class RPMColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            VerbProperties? verbProps = thingDef.TurretGunDefOrSelf().Verbs.Primary();

            if (verbProps is { Ranged: true, showBurstShotStats: true, burstShotCount: > 1 })
            {
                // Reminder: This is not IRL RPM.
                decimal cellValue = (60f / verbProps.ticksBetweenBurstShots.TicksToSeconds()).ToDecimal(0);

                return new NumberTableCell(cellValue, "0 rpm");
            }
        }

        return default;
    }
}
