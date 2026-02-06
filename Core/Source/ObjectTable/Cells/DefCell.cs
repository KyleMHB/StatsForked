using System;
using System.Collections.Generic;
using System.Linq;
using Stats.ObjectTable.FilterWidgets;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats.ObjectTable.Cells;

public sealed class DefCell : Cell
{
    private readonly Def? Value;
    public DefCell(Def value) : this(() => value) { }
    public DefCell(CellValueSource<Def?> valueSource)
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
    static private Def? GetValue(Cell cell)
    {
        return ((DefCell)cell).Value;
    }
    static private int CompareByDefLabel(Cell cell1, Cell cell2)
    {
        return Comparer<string?>.Default.Compare(GetValue(cell1)?.label, GetValue(cell2)?.label);
    }
    static public CellDescriptor GetDescriptor(ColumnDef columnDef, IEnumerable<Def?> defs) => GetDescriptor(columnDef.Title, defs);
    static public CellDescriptor GetDescriptor(Widget valueFieldLabel, IEnumerable<Def?> defs)
    {
        IEnumerable<NTMFilterOption<Def?>> valueFieldFilterOptions = defs
            .OrderBy(def => def?.label)
            .Select<Def?, NTMFilterOption<Def?>>(
                def => def == null ? new() : new(def, def.LabelCap)
            );
        FilterWidget valueFieldFilter = new OTMFilter<Def?>(GetValue, valueFieldFilterOptions);
        CellFieldDescriptor valueField = new(valueFieldLabel, valueFieldFilter, CompareByDefLabel);

        return new CellDescriptor(CellStyleType.String, [valueField]);
    }
    public static readonly DefCell Empty = new(() => null);
}
