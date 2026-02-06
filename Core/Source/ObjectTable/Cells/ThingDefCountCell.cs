using System;
using System.Collections.Generic;
using System.Linq;
using Stats.ObjectTable.FilterWidgets;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats.ObjectTable.Cells;

public class ThingDefCountCell : Cell
{
    private readonly ThingDef? ThingDef;
    private readonly decimal Count;
    public ThingDefCountCell(ThingDefCount value) : this(() => value) { }
    public ThingDefCountCell(CellValueSource<ThingDefCount> valueSource)
    {
        //ThingDef = thingDef;
        //Count = count;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        throw new NotImplementedException();
    }
    public override Vector2 GetSize()
    {
        throw new NotImplementedException();
    }
    public override void Refresh()
    {
    }
    static private decimal GetCount(Cell cell)
    {
        return ((ThingDefCountCell)cell).Count;
    }
    static private int CompareByCount(Cell cell1, Cell cell2)
    {
        return GetCount(cell1).CompareTo(GetCount(cell2));
    }
    static private ThingDef? GetThingDef(Cell cell)
    {
        return ((ThingDefCountCell)cell).ThingDef;
    }
    static private int CompareByThingDefLabel(Cell cell1, Cell cell2)
    {
        return Comparer<string?>.Default.Compare(GetThingDef(cell1)?.label, GetThingDef(cell2)?.label);
    }
    static public CellDescriptor GetDescriptor(IEnumerable<ThingDef?> thingDefs)
    {
        Widget countFieldLabel = new Label("Amount");
        FilterWidget countFilter = new NumberFilter(GetCount);
        CellFieldDescriptor countField = new(countFieldLabel, countFilter, CompareByCount);

        Widget thingDefFieldLabel = new Label("Type");
        IEnumerable<NTMFilterOption<ThingDef?>> thingDefFilterOptions = thingDefs
            .OrderBy(thingDef => thingDef?.label)
            .Select<ThingDef?, NTMFilterOption<ThingDef?>>(
                thingDef => thingDef == null
                    ? new()
                    : new(thingDef, thingDef.LabelCap, new ThingDefIcon(thingDef))
            );
        FilterWidget thingDefFilter = new OTMFilter<ThingDef?>(GetThingDef, thingDefFilterOptions);
        CellFieldDescriptor thingDefField = new(thingDefFieldLabel, thingDefFilter, CompareByThingDefLabel);

        return new CellDescriptor(CellStyleType.Number, [countField, thingDefField]);
    }
    public static readonly ThingDefCountCell Empty = new(ThingDefCount.Empty);
}

public readonly record struct ThingDefCount(ThingDef? ThingDef, decimal Count)
{
    public static readonly ThingDefCount Empty = new();
}
