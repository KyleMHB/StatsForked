using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats;

public sealed class Thing_EquippedStatOffsetsColumnWorker : ColumnWorker<ThingAlike>
{
    public Thing_EquippedStatOffsetsColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
    {
    }
    private static readonly Func<ThingDef, string> GetOffsetsString =
    FunctionExtensions.Memoized((ThingDef thingDef) =>
    {
        if (GetOffsetsCount(thingDef) == 0)
        {
            return "";
        }

        var result = new StringBuilder();

        foreach (var offset in thingDef.equippedStatOffsets)
        {
            result.Append(offset.stat.LabelCap);
            result.Append(GetOffsetValueString(offset));
        }

        return result.ToString();
    });
    private static int GetOffsetsCount(ThingDef thingDef)
    {
        return thingDef.equippedStatOffsets?.Count ?? 0;
    }
    private static string GetOffsetValueString(StatModifier offset)
    {
        return offset.stat.ValueToString(
            offset.value,
            ToStringNumberSense.Offset,
            offset.stat.finalizeEquippedStatOffset
        );
    }
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        if (GetOffsetsCount(thing.Def) == 0)
        {
            return null;
        }

        var labels = new StringBuilder();
        var values = new StringBuilder();

        foreach (var offset in thing.Def.equippedStatOffsets.OrderBy(offset => offset.stat.label))
        {
            var offsetLabel = $"{offset.stat.LabelCap}:";
            var offsetValueStr = GetOffsetValueString(offset);

            labels.AppendInNewLine(offsetLabel);
            values.AppendInNewLine(offsetValueStr);
        }

        return new HorizontalContainer(
            [
                new Label(labels.ToString())
                    .TextAnchor(TextAnchor.LowerLeft),
                new Label(values.ToString())
                    .WidthRel(1f)
                    .TextAnchor(TextAnchor.LowerRight),
            ],
            Globals.GUI.Pad,
            true
        );
    }
    public override FilterWidget<ThingAlike> GetFilterWidget(IEnumerable<ThingAlike> _)
    {
        return Make.StringFilter<ThingAlike>(thing => GetOffsetsString(thing.Def));
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetOffsetsCount(thing1.Def).CompareTo(GetOffsetsCount(thing2.Def));
    }
}
