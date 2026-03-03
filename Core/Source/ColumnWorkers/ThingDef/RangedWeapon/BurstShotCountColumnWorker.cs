using Stats.TableCells;
using Stats.TableWorkers;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.RangedWeapon;

public sealed class BurstShotCountColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        VerbProperties? verbProps = thingDef.TurretGunDefOrSelf().Verbs.Primary();

        if (verbProps is { Ranged: true, showBurstShotStats: true })
        {
            decimal cellValue = verbProps.burstShotCount;

            return new NumberCell.Constant(cellValue);
        }

        return NumberCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
