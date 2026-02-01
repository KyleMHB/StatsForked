using System.Collections.Generic;
using Stats;
using Stats.Objects.Thing.TableWorkers;
using Stats.Objects.Turret;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;
using Stats.ObjectTable.ColumnWorkers;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.RangedWeapon;

public sealed class RangedAimingTimeColumnWorker :
    IColumnWorker<RangedWeaponDef>,
    IColumnWorker<RangedWeaponThing>,
    IColumnWorker<TurretDef>
{
    public CellStyleType CellStyle { get; } = CellStyleType.Number;
    private readonly ColumnDef ColumnDef;
    private readonly string FormatString;
    public RangedAimingTimeColumnWorker(ColumnDef columnDef)
    {
        ColumnDef = columnDef;
        FormatString = "0.00 " + "LetterSecond".Translate();
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

        if (verbProps != null)
        {
            decimal cellValue = verbProps.warmupTime.ToDecimal(2);

            return new NumberCell(cellValue, FormatString);
        }

        return new NumberCell();
    }
    public IEnumerable<ColumnPart> GetCellDescriptor()
    {
        yield return new(ColumnDef.Title, new NumberFilter(NumberCell.GetValue), NumberCell.Compare);
    }
}
