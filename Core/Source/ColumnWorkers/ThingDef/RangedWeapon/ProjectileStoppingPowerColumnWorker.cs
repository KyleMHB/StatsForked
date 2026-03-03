using Stats.TableCells;
using Stats.TableWorkers;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.RangedWeapon;

public sealed class ProjectileStoppingPowerColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        ProjectileProperties? defaultProjProps = thingDef.TurretGunDefOrSelf().Verbs.Primary()?.defaultProjectile?.projectile;

        if (defaultProjProps != null)
        {
            decimal cellValue = defaultProjProps.stoppingPower.ToDecimal(1);

            return new NumberCell.Constant(cellValue, "0.0");
        }

        return NumberCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
