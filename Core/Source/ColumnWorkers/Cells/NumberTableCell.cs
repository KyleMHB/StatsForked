using Stats.Extensions;
using UnityEngine;
using Verse;

namespace Stats.ColumnWorkers.Cells;

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
            Widgets_Legacy.Draw.Label(rect, _text, GUISkin.TableCell.Number);
        }
    }
}
