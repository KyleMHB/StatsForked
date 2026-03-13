using Verse;
using Stats.ColumnWorkers.Cells;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.ThingDef.RangedWeapon;

public sealed class DirectHitChanceColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            VerbProperties? verbProps = thingDef.TurretGunDefOrSelf().Verbs.Primary();

            if (verbProps != null)
            {
                decimal cellValue = verbProps.ForcedMissRadius > 0f
                    ? (100f / GenRadial.NumCellsInRadius(verbProps.ForcedMissRadius)).ToDecimal(1)
                    : 100m;

                return new NumberTableCell(cellValue, "0.0\\%");
            }
        }

        return default;
    }
}
