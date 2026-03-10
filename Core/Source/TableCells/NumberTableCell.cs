using UnityEngine;
using Verse;

namespace Stats.TableCells;

public interface INumberTableCell : ITableCell
{
    public decimal Value { get; }
}

public readonly struct NumberTableCell : INumberTableCell
{
    public float Width { get; }
    public bool IsRefreshable => false;
    public decimal Value { get; }

    private readonly string? _text;

    public NumberTableCell(decimal value, string formatString = "")
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
        if (_text != null)
        {
            rect = rect.ContractedByObjectTableCellPadding();
            Widgets.Draw.Label(rect, _text, TableCellStyle.Number);
        }
    }
}
