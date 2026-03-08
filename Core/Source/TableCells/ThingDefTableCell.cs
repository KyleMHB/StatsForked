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
        if (Value != null)
        {
            rect = rect.ContractedByObjectTableCellPadding();

            Rect iconRect = rect.CutByX(_iconWidth);
            _icon!.DrawIn(iconRect);
            bool iconWasClicked = Widgets.Draw.ButtonGhostly(iconRect);

            if (iconWasClicked)
            {
                Widgets.Draw.DefInfoDialog(Value);
            }

            rect.CutByX(ObjectTableWidget.CellContentSpacing);

            Verse.Widgets.Label(rect, Text);
        }
    }
}
