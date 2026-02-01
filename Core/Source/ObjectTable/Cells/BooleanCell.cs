using System;
using Stats.ObjectTable.FilterWidgets;
using Stats.Widgets;
using UnityEngine;

namespace Stats.ObjectTable.Cells;

public sealed class BooleanCell : Cell
{
    private readonly bool Value;
    public BooleanCell(CellValueSource<bool> valueSource)
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
    static private bool GetValue(Cell cell)
    {
        return ((BooleanCell)cell).Value;
    }
    static private int Compare(Cell cell1, Cell cell2)
    {
        return GetValue(cell1).CompareTo(GetValue(cell2));
    }
    static public CellDescriptor GetDescriptor(Widget valueFieldLabel)
    {
        FilterWidget valueFieldFilter = new BooleanFilter(GetValue);
        CellFieldDescriptor valueField = new(valueFieldLabel, valueFieldFilter, Compare);

        return new CellDescriptor(CellStyleType.Boolean, [valueField]);
    }
}
