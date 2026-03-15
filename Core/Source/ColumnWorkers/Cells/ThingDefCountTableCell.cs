using Stats.Utils;
using Stats.Utils.Extensions;
using Stats.Utils.Widgets;
using UnityEngine;
using Verse;

namespace Stats.ColumnWorkers.Cells;

public interface IThingDefCountTableCell : ITableCell
{
    public Verse.ThingDef? ThingDef { get; }
    public string? ThingDefLabel { get; }
    public decimal Count { get; }
}

public readonly struct ThingDefCountTableCell : IThingDefCountTableCell
{
    public float Width { get; }
    public bool IsRefreshable => false;
    public Verse.ThingDef? ThingDef { get; }
    public string? ThingDefLabel { get; }
    public decimal Count { get; }

    private readonly string? _text;
    private readonly ThingDefIcon? _icon;
    private readonly float _iconWidth;
    private readonly TipSignal _tooltip;

    public ThingDefCountTableCell(Verse.ThingDef thingDef, decimal count)
    {
        ThingDef = thingDef;
        ThingDefLabel = thingDef.label;
        _tooltip = thingDef.LabelCap;
        Count = count;
        _text = count.ToString();
        float textWidth = _text.CalcSize(GUIStyles.TableCell.NumberNoPad).x;
        Vector2 iconSize = new(GUIStyles.Text.LineHeight, GUIStyles.Text.LineHeight);
        _iconWidth = iconSize.x;
        _icon = new ThingDefIcon(iconSize, thingDef);
        Width = textWidth + GUIStyles.TableCell.ContentSpacing + _iconWidth;
    }

    public void Draw(Rect rect)
    {
        if (ThingDef != null && Count != 0m)
        {
            rect = rect.ContractedByObjectTableCellPadding();

            Rect labelRect = rect.CutByX(rect.width - GUIStyles.TableCell.ContentSpacing - _iconWidth);
            if (Event.current.IsRepaint())
            {
                labelRect.Label(_text, GUIStyles.TableCell.NumberNoPad);
            }

            rect.xMin += GUIStyles.TableCell.ContentSpacing;

            if (Event.current.IsRepaint()) _icon!.Draw(rect);
            bool iconWasClicked = rect.ButtonGhostly();
            if (iconWasClicked)
            {
                ThingDef.OpenInfoDialog();
            }
            rect.Tip(_tooltip);
        }
    }
}
