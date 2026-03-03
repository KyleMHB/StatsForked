using Stats.TableCells;
using Stats.TableWorkers;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.RangedWeapon;

public sealed class RPMColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        VerbProperties? verbProps = thingDef.TurretGunDefOrSelf().Verbs.Primary();

        if (verbProps is { Ranged: true, showBurstShotStats: true, burstShotCount: > 1 })
        {
            // Reminder: This is not IRL RPM.
            decimal cellValue = (60f / verbProps.ticksBetweenBurstShots.TicksToSeconds()).ToDecimal(0);

            return new NumberCell.Constant(cellValue, "0 rpm");
        }

        return NumberCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
