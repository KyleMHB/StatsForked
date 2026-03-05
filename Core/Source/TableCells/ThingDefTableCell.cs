using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats.TableCells;

public interface IThingDefTableCell : ITableCell
{
    public ThingDef? Value { get; }
    public string? Text { get; }
}

public readonly struct ThingDefTableCell : IThingDefTableCell
{
    public float Width { get; }
    public bool IsRefreshable => false;
    public ThingDef? Value { get; }
    public string? Text { get; }

    private readonly Widget? _icon;
    private readonly float _iconWidth;

    public ThingDefTableCell(ThingDef value)
    {
        Value = value;
        Text = value.LabelCap;
        _icon = new ThingDefIcon(value);
        float textWidth = Verse.Text.CalcSize(Text).x;
        float iconWidth = _icon.GetSize().x;
        Width = iconWidth + ObjectTableWidget.CellContentSpacing + textWidth;
        _iconWidth = iconWidth;
    }

    public void Draw(Rect rect)
    {
        if (Value != null && Event.current.type == EventType.Repaint)
        {
            rect = rect.ContractedByObjectTableCellPadding();

            _icon!.DrawIn(rect.CutByX(_iconWidth));

            rect.CutByX(ObjectTableWidget.CellContentSpacing);

            TextAnchor textAnchor = Verse.Text.Anchor;
            Verse.Text.Anchor = (TextAnchor)TableCellStyleType.String;

            Verse.Widgets.Label(rect, Text);

            Verse.Text.Anchor = textAnchor;
        }
    }
}
