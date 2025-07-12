using System;
using System.Collections.Generic;
using RimWorld;
using Stats.Widgets;
using Verse;

namespace Stats.Compat.CE;

public sealed class Weapon_CaliberColumnWorker : ColumnWorker<ThingAlike>
{
    public Weapon_CaliberColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
    {
    }
    private static readonly Func<ThingAlike, string?> GetCaliberName =
    FunctionExtensions.Memoized((ThingAlike thing) =>
    {
        var thingDef = thing.Def.building?.turretGunDef ?? thing.Def;
        var statRequest = StatRequest.For(thingDef, null);

        if (StatDefOf.Caliber.Worker.ShouldShowFor(statRequest) == false)
        {
            return null;
        }

        return StatDefOf.Caliber.Worker.GetStatDrawEntryLabel(
            StatDefOf.Caliber,
            StatDefOf.Caliber.Worker.GetValue(statRequest),
            ToStringNumberSense.Absolute,
            statRequest
        );
    });
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var caliberName = GetCaliberName(thing);

        if (caliberName == null || caliberName.Length == 0)
        {
            return null;
        }

        var thingDef = thing.Def.building?.turretGunDef ?? thing.Def;
        var statRequest = StatRequest.For(thingDef, null);
        var tooltip = StatDefOf.Caliber.Worker.GetExplanationFull(
            statRequest,
            ToStringNumberSense.Absolute,
            StatDefOf.Caliber.Worker.GetValue(statRequest)
        );
        var widget = new Label(caliberName);

        if (tooltip?.Length > 0)
        {
            return widget.Tooltip(tooltip);
        }

        return widget;
    }
    public override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<ThingAlike> tableRecords)
    {
        yield return new(ColumnDef.Title, Make.OTMFilter(GetCaliberName, tableRecords));
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return Comparer<string?>.Default.Compare(GetCaliberName(thing1), GetCaliberName(thing2));
    }
}
