using Stats.ObjectTable.FilterWidgets;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats.ObjectTable.Cells;

public class BooleanCell
{
    private static readonly Texture2D _textureTrue = Verse.Widgets.CheckboxOnTex;

    public class Constant : Cell
    {
        public bool Value;

        public Constant(bool value)
        {
            Value = value;
            Size = new Vector2(Text.LineHeight, Text.LineHeight) + ObjectTableWidget.CellPad;
        }

        public override void Draw(Rect rect)
        {
            if (Value && Event.current.type == EventType.Repaint)
            {
                // TODO: Make it not take full available space.
                rect = rect.ContractedBy(ObjectTableWidget.CellPadHor, ObjectTableWidget.CellPadVer);
                Verse.Widgets.DrawTextureFitted(rect, _textureTrue, 1f);
            }
        }

        public override void Refresh()
        {
        }
    }

    public sealed class Variable : Constant
    {
        private readonly CellValueSource<bool> _valueSource;

        public Variable(CellValueSource<bool> valueSource) : base(false)
        {
            _valueSource = valueSource;
        }

        public override void Refresh()
        {
            Value = _valueSource();
        }
    }

    static private bool GetValue(Cell cell)
    {
        return ((Constant)cell).Value;
    }

    static private int Compare(Cell cell1, Cell cell2)
    {
        return GetValue(cell1).CompareTo(GetValue(cell2));
    }

    static public CellDescriptor GetDescriptor(ColumnDef columnDef) => GetDescriptor(columnDef.Title);

    static public CellDescriptor GetDescriptor(Widget valueFieldLabel)
    {
        FilterWidget valueFieldFilter = new BooleanFilter(GetValue);
        CellFieldDescriptor valueField = new(valueFieldLabel, valueFieldFilter, Compare);

        return new CellDescriptor(CellStyleType.Boolean, [valueField]);
    }

    public static readonly Constant True = new(true);

    public static readonly Constant False = new(false);

    public static readonly Constant Empty = False;
}
