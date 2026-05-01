using System.Collections.Generic;
using System.Linq;
using Stats.Utils;
using Stats.Utils.Extensions;
using Stats.Utils.Widgets;
using UnityEngine;
using Verse;
using static Stats.GUIStyles.TableCell;

namespace Stats.ColumnWorkers.Cells;

public interface IThingDefSetCell : ICell
{
    public IReadOnlyCollection<Verse.ThingDef?>? Value { get; }
}

public readonly struct ThingDefSetCell : IThingDefSetCell
{
    public float Width { get; }
    public bool IsRefreshable => false;
    public IReadOnlyCollection<Verse.ThingDef?>? Value { get; }

    private readonly Verse.ThingDef? _firstThingDef;
    private readonly string? _previewText;
    private readonly ThingDefIcon? _icon;
    private readonly float _iconWidth;
    private readonly TipSignal _tooltip;

    public ThingDefSetCell(IReadOnlyCollection<Verse.ThingDef?> value)
    {
        Value = value;
        Width = 0f;
        _firstThingDef = null;
        _previewText = null;
        _icon = null;
        _iconWidth = 0f;
        _tooltip = default;

        List<Verse.ThingDef> orderedDefs = value
            .Where(thingDef => thingDef != null)
            .Select(thingDef => thingDef!)
            .OrderBy(thingDef => thingDef.LabelCap.RawText)
            .ToList();

        _firstThingDef = orderedDefs.FirstOrDefault();
        if (_firstThingDef != null)
        {
            int hiddenCount = orderedDefs.Count - 1;
            _previewText = hiddenCount > 0
                ? $"{_firstThingDef.LabelCap} +{hiddenCount}"
                : _firstThingDef.LabelCap;
            _tooltip = string.Join("\n", orderedDefs.Select(thingDef => thingDef.LabelCap));
            _icon = new ThingDefIcon(_firstThingDef);
            _iconWidth = _icon.Size.x;
            float textWidth = _previewText.CalcSize(StringNoPad).x;
            Width = _iconWidth + ContentSpacing + textWidth;
        }
    }

    public void Draw(Rect rect)
    {
        if (_firstThingDef != null && _previewText != null)
        {
            rect
                .ContractedByObjectTableCellPadding()
                .CutLeft(out Rect iconRect, _iconWidth)
                .CutLeft(ContentSpacing)
                .TakeRest(out Rect labelRect);

            if (Event.current.type == EventType.Repaint)
            {
                _icon!.Draw(iconRect);
                _previewText.Draw(labelRect, StringNoPad);
                rect.Tip(_tooltip);
            }

            if (iconRect.ButtonGhostly())
            {
                _firstThingDef.OpenInfoDialog();
            }
        }
    }
}
