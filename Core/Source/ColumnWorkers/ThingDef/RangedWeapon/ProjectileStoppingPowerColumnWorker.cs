using Stats.TableCells;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.RangedWeapon;

public sealed class ProjectileStoppingPowerColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            ProjectileProperties? defaultProjProps = thingDef.TurretGunDefOrSelf().Verbs.Primary()?.defaultProjectile?.projectile;

            if (defaultProjProps != null)
            {
                decimal cellValue = defaultProjProps.stoppingPower.ToDecimal(1);

                return new NumberTableCell(cellValue, "0.0");
            }
        }

        return default;
    }
}
