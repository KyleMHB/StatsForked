using Stats.TableCells;
using Stats.TableWorkers;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.RangedWeapon;

public sealed class RangedAimingTimeColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    private readonly string FormatString = "0.00 " + "LetterSecond".Translate();
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        VerbProperties? verbProps = thingDef.TurretGunDefOrSelf().Verbs.Primary();

        if (verbProps != null)
        {
            decimal cellValue = verbProps.warmupTime.ToDecimal(2);

            return new NumberCell.Constant(cellValue, FormatString);
        }

        return NumberCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
