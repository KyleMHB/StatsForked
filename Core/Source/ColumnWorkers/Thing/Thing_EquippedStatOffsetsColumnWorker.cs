//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using RimWorld;
//using Stats.Widgets;
//using UnityEngine;
//using Verse;

//namespace Stats;

//public sealed class Thing_EquippedStatOffsetsColumnWorker : ColumnWorker<ThingAlike>
//{
//    public Thing_EquippedStatOffsetsColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
//    {
//    }
//    private static int GetStatModsCount(ThingDef thingDef)
//    {
//        return thingDef.equippedStatOffsets?.Count ?? 0;
//    }
//    private static string GetStatModValueString(StatModifier statMod)
//    {
//        return statMod.stat.ValueToString(
//            statMod.value,
//            ToStringNumberSense.Offset,
//            statMod.stat.finalizeEquippedStatOffset
//        );
//    }
//    public override Widget? GetTableCellWidget(ThingAlike thing)
//    {
//        if (GetStatModsCount(thing.Def) == 0)
//        {
//            return null;
//        }

//        var labels = new StringBuilder();
//        var values = new StringBuilder();

//        foreach (var statMod in thing.Def.equippedStatOffsets.OrderBy(statMod => statMod.stat.label))
//        {
//            var statModLabel = $"{statMod.stat.LabelCap}:";
//            var statModValueStr = GetStatModValueString(statMod);

//            labels.AppendInNewLine(statModLabel);
//            values.AppendInNewLine(statModValueStr);
//        }

//        return new HorizontalContainer([
//            new Label(labels.ToString())
//            .TextAnchor(TextAnchor.LowerLeft),

//            new Label(values.ToString())
//            .WidthRel(1f)
//            .TextAnchor(TextAnchor.LowerRight),
//        ], Globals.GUI.Pad, true);
//    }
//    public override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<ThingAlike> tableRecords)
//    {
//        yield return new(new Label("Has any"), Make.BooleanFilter<ThingAlike>(thing => GetStatModsCount(thing.Def) > 0));

//        var stats =
//            tableRecords
//            .Where(thing => thing.Def.equippedStatOffsets?.Count > 0)
//            .SelectMany(thing => thing.Def.equippedStatOffsets.Select(statMod => statMod.stat))
//            .Distinct();

//        foreach (var stat in stats.OrderBy(statDef => statDef.label))
//        {
//            var GetStatModValue = new Func<ThingDef, decimal>(thingDef =>
//            {
//                if (thingDef.equippedStatOffsets?.Count > 0)
//                {
//                    foreach (var statMod in thingDef.equippedStatOffsets)
//                    {
//                        if (statMod.stat == stat)
//                        {
//                            var label = statMod.ValueToStringAsOffset;
//                            var match = Thing_StatColumnWorker.NumberRegex.Match(label);

//                            if (match.Success)
//                            {
//                                return decimal.Parse(match.Groups[1].Captures[0].Value);
//                            }
//                        }
//                    }
//                }

//                return 0m;
//            }).Memoized();

//            var filterWidget = Make.NumberFilter<ThingAlike>(thing => GetStatModValue(thing.Def));

//            yield return new(new Label(stat.LabelCap), filterWidget);
//        }
//    }
//    public override int Compare(ThingAlike thing1, ThingAlike thing2)
//    {
//        return GetStatModsCount(thing1.Def).CompareTo(GetStatModsCount(thing2.Def));
//    }
//}
