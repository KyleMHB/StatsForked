using Stats.TableCells;
using Stats.TableWorkers;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.RangedWeapon;

public sealed class ProjectileDamageColumnWorker(ColumnDef columnDef) : StaticColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            ProjectileProperties? defaultProjProps = thingDef.TurretGunDefOrSelf().Verbs.Primary()?.defaultProjectile?.projectile;

            if (defaultProjProps?.damageDef?.harmsHealth == true)
            {
                decimal cellValue = defaultProjProps.GetDamageAmount(thingDef, null);

                return new NumberTableCell(cellValue);
            }
        }

        return default;
    }
    //public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberTableCell.GetDescriptor(columnDef);
}
