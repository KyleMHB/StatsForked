using Stats.TableCells;
using Stats.TableWorkers;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.RangedWeapon;

public sealed class ProjectileStoppingPowerColumnWorker(ColumnDef columnDef) : StaticColumnWorker<DefBasedObject, NumberTableCell>
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
    //public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberTableCell.GetDescriptor(columnDef);
}
