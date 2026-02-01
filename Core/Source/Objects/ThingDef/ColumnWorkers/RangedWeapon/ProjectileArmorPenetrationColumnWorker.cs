using System.Collections.Generic;
using Stats;
using Stats.Objects.Thing.TableWorkers;
using Stats.Objects.Turret;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.RangedWeapon;

public sealed class ProjectileArmorPenetrationColumnWorker :
    IColumnWorker<RangedWeaponDef>,
    IColumnWorker<RangedWeaponThing>,
    IColumnWorker<TurretDef>
{
    public CellStyleType CellStyle { get; } = CellStyleType.Number;
    private readonly ColumnDef ColumnDef;
    public ProjectileArmorPenetrationColumnWorker(ColumnDef columnDef)
    {
        ColumnDef = columnDef;
    }
    public Cell GetCell(RangedWeaponDef rangedWeaponDef)
    {
        return GetCell(rangedWeaponDef.Def);
    }
    public Cell GetCell(RangedWeaponThing rangedWeapon)
    {
        return GetCell(rangedWeapon.Thing.def);
    }
    public Cell GetCell(TurretDef turretDef)
    {
        return GetCell(turretDef.GunDef);
    }
    private Cell GetCell(ThingDef thingDef)
    {
        VerbProperties? verbProps = thingDef.Verbs.Primary();
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

        return new NumberCell();
    }
    public IEnumerable<ColumnPart> GetCellDescriptor()
    {
        yield return new(ColumnDef.Title, new NumberFilter(NumberCell.GetValue), NumberCell.Compare);
    }
}
