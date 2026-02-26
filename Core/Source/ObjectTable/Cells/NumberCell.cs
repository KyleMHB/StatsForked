using Stats.ObjectTable.FilterWidgets;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats.ObjectTable.Cells;

public static class NumberCell
{
    public class Constant : Cell
    {
        public decimal Value;

        protected string _text = "";

        public Constant(decimal value, string formatString = "")
        {
            Value = value;
            _text = value.ToString(formatString);
            Size = Text.CalcSize(_text) + ObjectTableWidget.CellPad;
        }

        public Constant()
        {
        }

        public override void Draw(Rect rect)
        {
            if (Value != 0m && Event.current.type == EventType.Repaint)
            {
                rect = rect.ContractedBy(ObjectTableWidget.CellPadHor, ObjectTableWidget.CellPadVer);

                TextAnchor textAnchor = Text.Anchor;
                Text.Anchor = (TextAnchor)CellStyleType.Number;

                Verse.Widgets.Label(rect, _text);

                Text.Anchor = textAnchor;
            }
        }

        public override void Refresh()
        {
        }
    }

    public sealed class Variable : Constant
    {
        private readonly CellValueSource<decimal> _valueSource;
        private readonly string _formatString;

        public Variable(CellValueSource<decimal> valueSource, string formatString = "")
        {
            _valueSource = valueSource;
            _formatString = formatString;
        }

        public override void Refresh()
        {
            decimal newValue = _valueSource();

            if (newValue != Value)
            {
                Value = newValue;
                // It may be a good idea to defer deriving view-related props
                // because we may not need them because the row may not pass the filters.
                _text = newValue.ToString(_formatString);
                Size = Text.CalcSize(_text) + ObjectTableWidget.CellPad;
            }
        }
    }

    static private decimal GetValue(Cell cell)
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
        FilterWidget valueFieldFilter = new NumberFilter(GetValue);
        CellFieldDescriptor valueField = new(valueFieldLabel, valueFieldFilter, Compare);

        return new CellDescriptor(CellStyleType.Number, [valueField]);
    }

    public static readonly Constant Empty = new();
}
