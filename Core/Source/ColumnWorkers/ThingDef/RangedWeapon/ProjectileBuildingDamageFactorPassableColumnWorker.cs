using Stats.TableCells;
using Stats.TableWorkers;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.RangedWeapon;

public sealed class ProjectileBuildingDamageFactorPassableColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        DamageDef? defaultProjDamageDef = thingDef.TurretGunDefOrSelf().Verbs.Primary()?.defaultProjectile?.projectile?.damageDef;

        if (defaultProjDamageDef != null)
        {
            decimal cellValue = (100f * defaultProjDamageDef.buildingDamageFactorPassable).ToDecimal(0);

            return new NumberCell.Constant(cellValue, "0\\%");
        }

        return NumberCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
