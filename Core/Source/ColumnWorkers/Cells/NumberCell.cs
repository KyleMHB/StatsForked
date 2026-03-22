using Stats.Utils;
using UnityEngine;

namespace Stats.ColumnWorkers.Cells;

public interface INumberCell : ICell
{
    public decimal Value { get; }
}

public readonly struct NumberCell : INumberCell
{
    public float Width { get; }
    public bool IsRefreshable => false;
    public decimal Value { get; }

    private readonly string? _text;

    public NumberCell(decimal value, string formatString = "")
    {
        Value = value;
        if (value != 0m)
        {
            _text = value.ToString(formatString);
            Width = _text.CalcSize(GUIStyles.TableCell.NumberNoPad).x;
        }
    }

    public void Draw(Rect rect)
    {
        if (_text != null)
        {
            rect.Label(_text, GUIStyles.TableCell.Number);
        }
    }
}
