using UnityEngine;

namespace Stats.ColumnWorkers.Cells;

public interface IDefTableCell : ITableCell
{
    public Verse.Def? Value { get; }
    public string? Text { get; }
}

public readonly struct DefTableCell : IDefTableCell
{
    public float Width { get; }
    public bool IsRefreshable => false;
    public Verse.Def? Value { get; }
    public string? Text { get; }

    public DefTableCell(Verse.Def value)
    {
        Value = value;
        Text = value.LabelCap;
        Width = Verse.Text.CalcSize(Text).x;
    }

    public void Draw(Rect rect)
    {
        if (Text != null)
        {
            Widgets_Legacy.Draw.Label(rect, Text, GUISkin.TableCell.String);
        }
    }
}
