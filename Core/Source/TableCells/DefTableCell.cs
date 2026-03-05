using UnityEngine;
using Verse;

namespace Stats.TableCells;

public interface IDefTableCell : ITableCell
{
    public Def? Value { get; }
    public string? Text { get; }
}

public readonly struct DefTableCell : IDefTableCell
{
    public float Width { get; }
    public bool IsRefreshable => false;
    public Def? Value { get; }
    public string? Text { get; }

    public DefTableCell(Def value)
    {
        Value = value;
        Text = value.LabelCap;
        Width = Verse.Text.CalcSize(Text).x;
    }

    public void Draw(Rect rect)
    {
        if (Value != null && Event.current.type == EventType.Repaint)
        {
            rect = rect.ContractedByObjectTableCellPadding();

            TextAnchor textAnchor = Verse.Text.Anchor;
            Verse.Text.Anchor = (TextAnchor)TableCellStyleType.String;

            Verse.Widgets.Label(rect, Text);

            Verse.Text.Anchor = textAnchor;
        }
    }
}
