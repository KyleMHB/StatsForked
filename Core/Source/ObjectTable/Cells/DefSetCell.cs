using System;
using System.Collections.Generic;
using System.Linq;
using Stats.ObjectTable.FilterWidgets;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats.ObjectTable.Cells;

public class DefSetCell : Cell
{
    private IReadOnlyCollection<Def?> Value;
    private string Text = "";
    public DefSetCell(IReadOnlyCollection<Def?> value) : this(() => value) { }
    public DefSetCell(CellValueSource<IReadOnlyCollection<Def?>> valueSource)
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
    static private IReadOnlyCollection<Def?> GetValue(Cell cell)
    {
        return ((DefSetCell)cell).Value;
    }
    static private int CompareByCellText(Cell cell1, Cell cell2)
    {
        return ((DefSetCell)cell1).Text.CompareTo(((DefSetCell)cell2).Text);
    }
    static public CellDescriptor GetDescriptor(ColumnDef columnDef, IEnumerable<Def?> defs) => GetDescriptor(columnDef.Title, defs);
    static public CellDescriptor GetDescriptor(Widget valueFieldLabel, IEnumerable<Def?> defs)
    {
        IEnumerable<NTMFilterOption<Def?>> valueFieldFilterOptions = defs
            .OrderBy(def => def?.label)
            .Select<Def?, NTMFilterOption<Def?>>(
                def => def == null ? new() : new(def, def.LabelCap)
            );
        FilterWidget valueFieldFilter = new MTMFilter<Def?>(GetValue, valueFieldFilterOptions);
        CellFieldDescriptor valueField = new(valueFieldLabel, valueFieldFilter, CompareByCellText);

        return new CellDescriptor(CellStyleType.String, [valueField]);
    }
    public static readonly DefSetCell Empty = new([]);
}
