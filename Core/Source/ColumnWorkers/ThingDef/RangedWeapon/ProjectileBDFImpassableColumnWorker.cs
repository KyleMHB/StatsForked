using Verse;
using Stats.ColumnWorkers.Cells;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.ThingDef.RangedWeapon;

public sealed class ProjectileBDFImpassableColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            DamageDef? defaultProjDamageDef = thingDef.TurretGunDefOrSelf().Verbs.Primary()?.defaultProjectile?.projectile?.damageDef;

            if (defaultProjDamageDef != null)
            {
                decimal cellValue = (100f * defaultProjDamageDef.buildingDamageFactorImpassable).ToDecimal(0);

                return new NumberCell(cellValue, "0\\%");
            }
        }

        return default;
    }
}
