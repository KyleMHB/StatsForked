using System;
using Stats.ObjectTable.FilterWidgets;
using Stats.Widgets;
using UnityEngine;

namespace Stats.ObjectTable.Cells;

public sealed class NumberCell : Cell
{
    private readonly CellValueSource<decimal> ValueSource;
    private decimal Value;
    private readonly string FormatString;
    private string Text = "";
    private Vector2 Size;
    public NumberCell(CellValueSource<decimal> valueSource, string formatString)
    {
        ValueSource = valueSource;
        FormatString = formatString;
    }
    //public NumberCell(decimal value) : this(() => value, "")
    //{
    //}
    //public NumberCell() : this(0m)
    //{
    //}
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
        decimal newValue = ValueSource();

        if (newValue != Value)
        {
            Value = newValue;
            // It may be a good idea to defer deriving view-related props
            // because we may not need them because the row may not pass the filters.
            Text = newValue.ToString(FormatString);
            // etc...
        }
    }
    static private decimal GetValue(Cell cell)
    {
        return ((NumberCell)cell).Value;
    }
    static private int Compare(Cell cell1, Cell cell2)
    {
        return GetValue(cell1).CompareTo(GetValue(cell2));
    }
    static public CellDescriptor GetDescriptor(Widget valueFieldLabel)
    {
        FilterWidget valueFieldFilter = new NumberFilter(GetValue);
        CellFieldDescriptor valueField = new(valueFieldLabel, valueFieldFilter, Compare);

        return new CellDescriptor(CellStyleType.Number, [valueField]);
    }
}
