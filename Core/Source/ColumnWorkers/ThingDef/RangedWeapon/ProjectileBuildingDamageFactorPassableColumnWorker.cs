using Stats.TableCells;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.RangedWeapon;

public sealed class ProjectileBuildingDamageFactorPassableColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            DamageDef? defaultProjDamageDef = thingDef.TurretGunDefOrSelf().Verbs.Primary()?.defaultProjectile?.projectile?.damageDef;

            if (defaultProjDamageDef != null)
            {
                decimal cellValue = (100f * defaultProjDamageDef.buildingDamageFactorPassable).ToDecimal(0);

                return new NumberTableCell(cellValue, "0\\%");
            }
        }

        return default;
    }
}
