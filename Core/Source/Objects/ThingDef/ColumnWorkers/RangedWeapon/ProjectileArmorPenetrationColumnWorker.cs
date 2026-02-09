using Stats.ObjectTable;
using Stats.ObjectTable.Cells;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.RangedWeapon;

public sealed class ProjectileArmorPenetrationColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        VerbProperties? verbProps = thingDef.TurretGunDefOrSelf().Verbs.Primary();
        ProjectileProperties? defaultProjProps = verbProps?.defaultProjectile?.projectile;
        const string formatString = "0\\%";

        if (defaultProjProps?.damageDef is { harmsHealth: true, armorCategory: not null })
        {
            decimal cellValue = (defaultProjProps.GetArmorPenetration(null) * 100f).ToDecimal(0);

            return new NumberCell(cellValue, formatString);
        }
        else if (defaultProjProps == null && verbProps?.beamDamageDef != null)
        {
            decimal cellValue = (verbProps.beamDamageDef.defaultArmorPenetration * 100f).ToDecimal(0);

            return new NumberCell(cellValue, formatString);
        }

        return NumberCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
