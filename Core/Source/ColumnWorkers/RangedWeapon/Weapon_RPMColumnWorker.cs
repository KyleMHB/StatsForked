using System.Collections.Generic;
using Stats.Widgets;
using Verse;

namespace Stats;

public sealed class Weapon_RPMColumnWorker :
    IColumnWorker<RangedWeaponDef>,
    IColumnWorker<TurretDef>
{
    public IColumnWorker.CellStyleType CellStyle { get; } = IColumnWorker.CellStyleType.Number;
    private readonly ColumnDef Def;
    public Weapon_RPMColumnWorker(ColumnDef columnDef)
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
        yield return new(Def.Title, new NumberFilter(ConstTableCell<decimal>.GetValue));
    }
    private static ObjectTable.Cell GetCell(ThingDef thingDef)
    {
        return Make.ConstNumberTableCell(GetRPM(thingDef), "0 rpm");
    }
    private static decimal GetRPM(ThingDef thingDef)
    {
        var verb = thingDef.Verbs.Primary();

        // Reminder: This is not IRL RPM.
        if (verb is { Ranged: true, showBurstShotStats: true, burstShotCount: > 1 })
        {
            return (60f / verb.ticksBetweenBurstShots.TicksToSeconds()).ToDecimal(0);
        }

        return 0m;
    }
}
