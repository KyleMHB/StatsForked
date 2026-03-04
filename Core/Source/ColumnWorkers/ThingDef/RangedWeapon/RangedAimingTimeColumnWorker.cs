using Stats.TableCells;
using Stats.TableWorkers;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.RangedWeapon;

public sealed class RangedAimingTimeColumnWorker(ColumnDef columnDef) : StaticColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    private static readonly string FormatString = "0.00 " + "LetterSecond".Translate();

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            VerbProperties? verbProps = thingDef.TurretGunDefOrSelf().Verbs.Primary();

            if (verbProps != null)
            {
                decimal cellValue = verbProps.warmupTime.ToDecimal(2);

                return new NumberTableCell(cellValue, FormatString);
            }
        }

        return default;
    }
    //public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberTableCell.GetDescriptor(columnDef);
}
