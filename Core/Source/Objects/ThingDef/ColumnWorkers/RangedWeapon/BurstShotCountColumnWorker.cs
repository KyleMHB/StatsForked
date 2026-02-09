using Stats.ObjectTable;
using Stats.ObjectTable.Cells;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.RangedWeapon;

public sealed class BurstShotCountColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        VerbProperties? verbProps = thingDef.TurretGunDefOrSelf().Verbs.Primary();

        if (verbProps is { Ranged: true, showBurstShotStats: true })
        {
            decimal cellValue = verbProps.burstShotCount;

            return new NumberCell(cellValue);
        }

        return NumberCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
