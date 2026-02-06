using Stats.ObjectTable;
using Stats.ObjectTable.Cells;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.RangedWeapon;

public sealed class RangedMissRadiusColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell GetCell(Verse.ThingDef thingDef)
    {
        VerbProperties? verbProps = thingDef.TurretGunDefOrSelf().Verbs.Primary();

        if (verbProps != null)
        {
            decimal cellValue = verbProps.ForcedMissRadius.ToDecimal(1);

            return new NumberCell(cellValue, "0.0");
        }

        return NumberCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor() => NumberCell.GetDescriptor(columnDef);
}
