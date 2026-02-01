using System.Collections.Generic;
using Stats;
using Stats.Objects.Thing.TableWorkers;
using Stats.Objects.Turret;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;
using Stats.ObjectTable.ColumnWorkers;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.RangedWeapon;

public sealed class ProjectileStoppingPowerColumnWorker :
    IColumnWorker<RangedWeaponDef>,
    IColumnWorker<RangedWeaponThing>,
    IColumnWorker<TurretDef>
{
    public CellStyleType CellStyle { get; } = CellStyleType.Number;
    private readonly ColumnDef ColumnDef;
    public ProjectileStoppingPowerColumnWorker(ColumnDef columnDef)
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
        ProjectileProperties? defaultProjProps = thingDef.Verbs.Primary()?.defaultProjectile?.projectile;

        if (defaultProjProps != null)
        {
            decimal cellValue = defaultProjProps.stoppingPower.ToDecimal(1);

            return new NumberCell(cellValue, "0.0");
        }

        return new NumberCell();
    }
    public IEnumerable<ColumnPart> GetCellDescriptor()
    {
        yield return new(ColumnDef.Title, new NumberFilter(NumberCell.GetValue), NumberCell.Compare);
    }
}
