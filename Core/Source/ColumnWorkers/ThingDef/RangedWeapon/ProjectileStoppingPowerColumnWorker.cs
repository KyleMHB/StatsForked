using Verse;
using Stats.ColumnWorkers.Cells;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.ThingDef.RangedWeapon;

public sealed class ProjectileStoppingPowerColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            ProjectileProperties? defaultProjProps = thingDef.TurretGunDefOrSelf().Verbs.Primary()?.defaultProjectile?.projectile;

            if (defaultProjProps != null)
            {
                decimal cellValue = defaultProjProps.stoppingPower.ToDecimal(1);

                return new NumberCell(cellValue, "0.0");
            }
        }

        return default;
    }
}
