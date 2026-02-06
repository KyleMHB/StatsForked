using Stats.ObjectTable;
using Stats.ObjectTable.Cells;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.RangedWeapon;

public sealed class ProjectileBuildingDamageFactorPassableColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell GetCell(Verse.ThingDef thingDef)
    {
        DamageDef? defaultProjDamageDef = thingDef.TurretGunDefOrSelf().Verbs.Primary()?.defaultProjectile?.projectile?.damageDef;

        if (defaultProjDamageDef != null)
        {
            decimal cellValue = (100f * defaultProjDamageDef.buildingDamageFactorPassable).ToDecimal(0);

            return new NumberCell(cellValue, "0\\%");
        }

        return NumberCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor() => NumberCell.GetDescriptor(columnDef);
}
