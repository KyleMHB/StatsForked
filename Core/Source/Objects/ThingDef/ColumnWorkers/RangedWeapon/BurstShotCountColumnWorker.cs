using Stats;
using Stats.Objects.Thing.TableWorkers;
using Stats.Objects.Turret;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;
using Stats.ObjectTable.TableWorkers;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.RangedWeapon;

public sealed class BurstShotCountColumnWorker(ColumnDef ColumnDef) :
    IColumnWorker<RangedWeaponDef>,
    IColumnWorker<RangedWeaponThing>,
    IColumnWorker<TurretDef>
{
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

        if (verbProps is { Ranged: true, showBurstShotStats: true })
        {
            decimal cellValue = verbProps.burstShotCount;

            return new NumberCell(cellValue);
        }

        return new NumberCell();
    }
    public CellDescriptor GetCellDescriptor()
    {
        return NumberCell.GetDescriptor(ColumnDef.Title);
    }
}
