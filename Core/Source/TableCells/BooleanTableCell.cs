using RimWorld;
using Stats.FilterWidgets;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats.TableCells;

public readonly struct BooleanTableCell : ITableCell
{
    private static readonly Texture2D _textureTrue = Verse.Widgets.CheckboxOnTex;

    public float Width { get; }
    public readonly bool Value;

    public BooleanTableCell(bool value)
    {
        Value = value;
        Width = Text.LineHeight;
    }

    public void Draw(Rect rect)
    {
        if (Value && Event.current.type == EventType.Repaint)
        {
            // TODO: Make it not take full available space.
            rect = rect.ContractedByObjectTableCellPadding();
            Verse.Widgets.DrawTextureFitted(rect, _textureTrue, 1f);
        }
    }

    //static private int Compare(Cell cell1, Cell cell2)
    //{
    //    return GetValue(cell1).CompareTo(GetValue(cell2));
    //}

    //static public CellDescriptor GetDescriptor(ColumnDef columnDef) => GetDescriptor(columnDef.Title);

    //static public CellDescriptor GetDescriptor(Widget valueFieldLabel)
    //{
    //    FilterWidget valueFieldFilter = new BooleanFilter(GetValue);
    //    CellFieldDescriptor valueField = new(valueFieldLabel, valueFieldFilter, Compare);

    //    return new CellDescriptor(CellStyleType.Boolean, [valueField]);
    //}
}
