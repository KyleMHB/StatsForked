using Stats.Extensions;
using Stats.Utils;
using Stats.Widgets_Legacy;
using UnityEngine;

namespace Stats.ColumnWorkers.Cells;

public interface IThingDefTableCell : ITableCell
{
    public Verse.ThingDef? Value { get; }
    public string? Text { get; }
}

public readonly struct ThingDefTableCell : IThingDefTableCell
{
    public float Width { get; }
    public bool IsRefreshable => false;
    public Verse.ThingDef? Value { get; }
    public string? Text { get; }

    private readonly Widget? _icon;
    private readonly float _iconWidth;

    public ThingDefTableCell(Verse.ThingDef value)
    {
        Value = value;
        Text = value.LabelCap;
        _icon = new ThingDefIcon(value);
        float textWidth = Verse.Text.CalcSize(Text).x;
        float iconWidth = _icon.GetSize().x;
        Width = iconWidth + GUISkin.TableCell.ContentSpacing + textWidth;
        _iconWidth = iconWidth;
    }

    public void Draw(Rect rect)
    {
        if (Value != null)
        {
            rect = rect.ContractedByObjectTableCellPadding();

            Rect iconRect = rect.CutByX(_iconWidth);
            _icon!.DrawIn(iconRect);
            bool iconWasClicked = iconRect.ButtonGhostly();
            if (iconWasClicked)
            {
                Value.OpenInfoDialog();
            }

            if (Event.current.type == EventType.Repaint)
            {
                rect.CutByX(GUISkin.TableCell.ContentSpacing);

                Widgets_Legacy.Draw.Label(rect, Text, GUISkin.TableCell.String);
            }
        }
    }
}
