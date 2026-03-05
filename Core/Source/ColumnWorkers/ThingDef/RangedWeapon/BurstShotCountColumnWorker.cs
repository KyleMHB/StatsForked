using Stats.TableCells;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.RangedWeapon;

public sealed class BurstShotCountColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            VerbProperties? verbProps = thingDef.TurretGunDefOrSelf().Verbs.Primary();

            if (verbProps is { Ranged: true, showBurstShotStats: true })
            {
                decimal cellValue = verbProps.burstShotCount;

                return new NumberTableCell(cellValue);
            }
        }

        return default;
    }
}
