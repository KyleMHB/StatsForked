using System.Collections.Generic;
using Stats;
using Stats.Objects.Thing.TableWorkers;
using Stats.Objects.Turret;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.RangedWeapon;

public sealed class RPMColumnWorker :
    IColumnWorker<RangedWeaponDef>,
    IColumnWorker<RangedWeaponThing>,
    IColumnWorker<TurretDef>
{
    public CellStyleType CellStyle { get; } = CellStyleType.Number;
    private readonly ColumnDef ColumnDef;
    public RPMColumnWorker(ColumnDef columnDef)
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

        if (verbProps is { Ranged: true, showBurstShotStats: true, burstShotCount: > 1 })
        {
            // Reminder: This is not IRL RPM.
            decimal cellValue = (60f / verbProps.ticksBetweenBurstShots.TicksToSeconds()).ToDecimal(0);

            return new NumberCell(cellValue, "0 rpm");
        }

        return new NumberCell();
    }
    public IEnumerable<ColumnPart> GetCellDescriptor()
    {
        yield return new(ColumnDef.Title, new NumberFilter(NumberCell.GetValue), NumberCell.Compare);
    }
}
