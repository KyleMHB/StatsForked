using Stats.ObjectTable;
using Stats.ObjectTable.Cells;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.RangedWeapon;

public sealed class RangedAimingTimeColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    private readonly string FormatString = "0.00 " + "LetterSecond".Translate();
    public override Cell GetCell(Verse.ThingDef thingDef)
    {
        VerbProperties? verbProps = thingDef.TurretGunDefOrSelf().Verbs.Primary();

        if (verbProps != null)
        {
            decimal cellValue = verbProps.warmupTime.ToDecimal(2);

            return new NumberCell(cellValue, FormatString);
        }

        return NumberCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor() => NumberCell.GetDescriptor(columnDef);
}
