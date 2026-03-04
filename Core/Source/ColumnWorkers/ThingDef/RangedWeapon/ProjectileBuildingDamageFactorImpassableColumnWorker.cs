using Stats.TableCells;
using Stats.TableWorkers;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.RangedWeapon;

public sealed class ProjectileBuildingDamageFactorImpassableColumnWorker(ColumnDef columnDef) : StaticColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            DamageDef? defaultProjDamageDef = thingDef.TurretGunDefOrSelf().Verbs.Primary()?.defaultProjectile?.projectile?.damageDef;

            if (defaultProjDamageDef != null)
            {
                decimal cellValue = (100f * defaultProjDamageDef.buildingDamageFactorImpassable).ToDecimal(0);

                return new NumberTableCell(cellValue, "0\\%");
            }
        }

        return default;
    }
    //public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberTableCell.GetDescriptor(columnDef);
}
