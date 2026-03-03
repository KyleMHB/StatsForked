using Stats.TableCells;
using Stats.TableWorkers;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.RangedWeapon;

public sealed class RangedDirectHitChanceColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        VerbProperties? verbProps = thingDef.TurretGunDefOrSelf().Verbs.Primary();

        if (verbProps != null)
        {
            decimal cellValue = verbProps.ForcedMissRadius > 0f
                ? (100f / GenRadial.NumCellsInRadius(verbProps.ForcedMissRadius)).ToDecimal(1)
                : 100m;

            return new NumberCell.Constant(cellValue, "0.0\\%");
        }

        return NumberCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
