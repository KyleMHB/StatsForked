using System.Drawing;
using RimWorld;
using Stats.FilterWidgets;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats.TableCells;

public readonly struct NumberCell : ITableCell
{
    public float Width { get; }
    public readonly decimal Value;

    private readonly string _text = "";

    public NumberCell(decimal value, string formatString = "")
    {
        Value = value;
        if (value != 0m)
        {
            _text = value.ToString(formatString);
            Width = Text.CalcSize(_text).x;
        }
    }

    public void Draw(Rect rect)
    {
        if (Value != 0m && Event.current.type == EventType.Repaint)
        {
            rect = rect.ContractedByObjectTableCellPadding();

            TextAnchor textAnchor = Text.Anchor;
            Text.Anchor = (TextAnchor)TableCellStyleType.Number;

            Verse.Widgets.Label(rect, _text);

            Text.Anchor = textAnchor;
        }
    }

    //static private int Compare(Cell cell1, Cell cell2)
    //{
    //    return GetValue(cell1).CompareTo(GetValue(cell2));
    //}

    //static public CellDescriptor GetDescriptor(ColumnDef columnDef) => GetDescriptor(columnDef.Title);

    //static public CellDescriptor GetDescriptor(Widget valueFieldLabel)
    //{
    //    FilterWidget valueFieldFilter = new NumberFilter(GetValue);
    //    CellFieldDescriptor valueField = new(valueFieldLabel, valueFieldFilter, Compare);

    //    return new CellDescriptor(CellStyleType.Number, [valueField]);
    //}
}
