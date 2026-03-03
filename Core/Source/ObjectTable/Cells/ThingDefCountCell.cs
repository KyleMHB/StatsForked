using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.ObjectTable.FilterWidgets;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats.ObjectTable.Cells;

public readonly struct ThingDefCountCell : ICell
{
    public float Width { get; }
    public readonly ThingDef? ThingDef;
    public readonly decimal Count;

    public ThingDefCountCell(ThingDef thingDef, decimal count)
    {
        ThingDef = thingDef;
        Count = count;
    }

    public void Draw(Rect rect)
    {
        if (ThingDef != null && Event.current.type == EventType.Repaint)
        {
            rect = rect.ContractedByObjectTableCellPadding();

            TextAnchor textAnchor = Text.Anchor;
            Text.Anchor = (TextAnchor)CellStyleType.Number;

            Verse.Widgets.Label(rect, "TODO");

            Text.Anchor = textAnchor;
        }
    }

    //static private decimal GetCount(Cell cell)
    //{
    //    return ((ThingDefCountCell)cell).Count;
    //}
    //static private int CompareByCount(Cell cell1, Cell cell2)
    //{
    //    return GetCount(cell1).CompareTo(GetCount(cell2));
    //}
    //static private ThingDef? GetThingDef(Cell cell)
    //{
    //    return ((ThingDefCountCell)cell).ThingDef;
    //}
    //static private int CompareByThingDefLabel(Cell cell1, Cell cell2)
    //{
    //    return Comparer<string?>.Default.Compare(GetThingDef(cell1)?.label, GetThingDef(cell2)?.label);
    //}
    //static public CellDescriptor GetDescriptor(IEnumerable<ThingDef?> thingDefs)
    //{
    //    Widget countFieldLabel = new Label("Amount");
    //    FilterWidget countFilter = new NumberFilter(GetCount);
    //    CellFieldDescriptor countField = new(countFieldLabel, countFilter, CompareByCount);

    //    Widget thingDefFieldLabel = new Label("Type");
    //    IEnumerable<NTMFilterOption<ThingDef?>> thingDefFilterOptions = thingDefs
    //        .OrderBy(thingDef => thingDef?.label)
    //        .Select<ThingDef?, NTMFilterOption<ThingDef?>>(
    //            thingDef => thingDef == null
    //                ? new()
    //                : new(thingDef, thingDef.LabelCap, new ThingDefIcon(thingDef))
    //        );
    //    FilterWidget thingDefFilter = new OTMFilter<ThingDef?>(GetThingDef, thingDefFilterOptions);
    //    CellFieldDescriptor thingDefField = new(thingDefFieldLabel, thingDefFilter, CompareByThingDefLabel);

    //    return new CellDescriptor(CellStyleType.Number, [countField, thingDefField]);
    //}
}
