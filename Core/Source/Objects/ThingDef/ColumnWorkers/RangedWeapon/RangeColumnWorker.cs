using Stats.ObjectTable;
using Stats.ObjectTable.Cells;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.RangedWeapon;

public sealed class RangeColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell GetCell(Verse.ThingDef thingDef)
    {
        VerbProperties? verbProps = thingDef.TurretGunDefOrSelf().Verbs.Primary();

        if (verbProps != null)
        {
            decimal cellValue = verbProps.range.ToDecimal(0);

            return new NumberCell(cellValue);
        }

        return NumberCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor() => NumberCell.GetDescriptor(columnDef);
}
