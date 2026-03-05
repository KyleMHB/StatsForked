using Stats.TableCells;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.RangedWeapon;

public sealed class ProjectileArmorPenetrationColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            VerbProperties? verbProps = thingDef.TurretGunDefOrSelf().Verbs.Primary();
            ProjectileProperties? defaultProjProps = verbProps?.defaultProjectile?.projectile;
            const string formatString = "0\\%";

            if (defaultProjProps?.damageDef is { harmsHealth: true, armorCategory: not null })
            {
                decimal cellValue = (defaultProjProps.GetArmorPenetration(null) * 100f).ToDecimal(0);

                return new NumberTableCell(cellValue, formatString);
            }
            else if (defaultProjProps == null && verbProps?.beamDamageDef != null)
            {
                decimal cellValue = (verbProps.beamDamageDef.defaultArmorPenetration * 100f).ToDecimal(0);

                return new NumberTableCell(cellValue, formatString);
            }
        }

        return default;
    }
}
