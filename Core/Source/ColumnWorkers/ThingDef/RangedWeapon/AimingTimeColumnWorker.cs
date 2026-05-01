using Verse;
using Stats.ColumnWorkers.Cells;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.ThingDef.RangedWeapon;

public sealed class AimingTimeColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberCell>
{
    public override ColumnDef Def => columnDef;

    private static readonly string FormatString = "0.00 " + "LetterSecond".Translate();

    protected override NumberCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            VerbProperties? verbProps = thingDef.TurretGunDefOrSelf().Verbs.Primary();

            if (verbProps != null)
            {
                decimal cellValue = verbProps.warmupTime.ToDecimal(2);

                return new NumberCell(cellValue, FormatString);
            }
        }

        return default;
    }
}
