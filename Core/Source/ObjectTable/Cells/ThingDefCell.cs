using System;
using System.Collections.Generic;
using System.Linq;
using Stats.ObjectTable.FilterWidgets;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats.ObjectTable.Cells;

public class ThingDefCell : Cell
{
    private readonly ThingDef? Value;
    public ThingDefCell(ThingDef value) : this(() => value) { }
    public ThingDefCell(CellValueSource<ThingDef?> valueSource)
    {
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
    static private ThingDef? GetValue(Cell cell)
    {
        return ((ThingDefCell)cell).Value;
    }
    static private int CompareByDefLabel(Cell cell1, Cell cell2)
    {
        return Comparer<string?>.Default.Compare(GetValue(cell1)?.label, GetValue(cell2)?.label);
    }
    static public CellDescriptor GetDescriptor(ColumnDef columnDef, IEnumerable<ThingDef?> defs) => GetDescriptor(columnDef.Title, defs);
    static public CellDescriptor GetDescriptor(Widget valueFieldLabel, IEnumerable<ThingDef?> defs)
    {
        IEnumerable<NTMFilterOption<ThingDef?>> valueFieldFilterOptions = defs
            .OrderBy(def => def?.label)
            .Select<ThingDef?, NTMFilterOption<ThingDef?>>(
                def => def == null ? new() : new(def, def.LabelCap)
            );
        FilterWidget valueFieldFilter = new OTMFilter<ThingDef?>(GetValue, valueFieldFilterOptions);
        CellFieldDescriptor valueField = new(valueFieldLabel, valueFieldFilter, CompareByDefLabel);

        return new CellDescriptor(CellStyleType.String, [valueField]);
    }
    public static readonly ThingDefCell Empty = new(() => null);
}
