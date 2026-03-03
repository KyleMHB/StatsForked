using Stats.TableCells;
using Stats.TableWorkers;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.RangedWeapon;

public sealed class ProjectileDamageColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        ProjectileProperties? defaultProjProps = thingDef.TurretGunDefOrSelf().Verbs.Primary()?.defaultProjectile?.projectile;

        if (defaultProjProps?.damageDef?.harmsHealth == true)
        {
            decimal cellValue = defaultProjProps.GetDamageAmount(thingDef, null);

            return new NumberCell.Constant(cellValue);
        }

        return NumberCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
