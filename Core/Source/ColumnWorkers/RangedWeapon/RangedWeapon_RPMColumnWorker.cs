using System.Collections.Generic;
using Stats.Widgets;
using Verse;

namespace Stats;

public sealed class RangedWeapon_RPMColumnWorker :
    IColumnWorker<RangedWeaponDef>,
    IColumnWorker<TurretDef>
{
    public IColumnWorker.CellStyleType CellStyle { get; } = IColumnWorker.CellStyleType.Number;
    private readonly ColumnDef Def;
    public RangedWeapon_RPMColumnWorker(ColumnDef columnDef)
    {
        Def = columnDef;
    }
    public ObjectTable.Cell GetCell(RangedWeaponDef rangedWeaponDef)
    {
        return GetCell(rangedWeaponDef.Def);
    }
    public ObjectTable.Cell GetCell(TurretDef turretDef)
    {
        return GetCell(turretDef.GunDef);
    }
    public IEnumerable<ObjectTable.ObjectProp> GetObjectProps()
    {
        yield return new(Def.Title, new NumberFilter(AbstractConstTableCell<decimal>.GetValue));
    }
    private static ObjectTable.Cell GetCell(ThingDef thingDef)
    {
        var verb = thingDef.Verbs.Primary();

        if (verb is { Ranged: true, showBurstShotStats: true, burstShotCount: > 1 })
        {
            // Reminder: This is not IRL RPM.
            var rpm = (60f / verb.ticksBetweenBurstShots.TicksToSeconds()).ToDecimal(0);
            var rpmString = rpm.ToString("0 rpm");
            var cellWidget = new Label(rpmString).PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer);

            return new ConstTableCell<decimal>(rpm, cellWidget);
        }

        return new EmptyConstTableCell<decimal>(0m);
    }
}
