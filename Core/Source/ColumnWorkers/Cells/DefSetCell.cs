using System.Collections.Generic;
using System.Linq;
using Stats.Utils;
using UnityEngine;
using Verse;

namespace Stats.ColumnWorkers.Cells;

public interface IDefSetCell : ICell
{
    public IReadOnlyCollection<Verse.Def>? Value { get; }
    public string? Text { get; }
}

public readonly struct DefSetCell : IDefSetCell
{
    public float Width { get; }
    public bool IsRefreshable => false;
    public IReadOnlyCollection<Verse.Def>? Value { get; }
    public string? Text { get; }

    private readonly TipSignal _tooltip;
    private readonly string? _expandedText;

    public DefSetCell(IReadOnlyCollection<Verse.Def> value)
    {
        Value = value;
        Width = 0f;
        Text = null;
        _tooltip = default;
        _expandedText = null;

        List<Verse.Def> orderedDefs = value
            .OrderBy(def => def.LabelCap.RawText)
            .ToList();
        if (orderedDefs.Count > 0)
        {
            Verse.Def firstDef = orderedDefs[0];
            int hiddenCount = orderedDefs.Count - 1;
            Text = hiddenCount > 0
                ? $"{firstDef.LabelCap} +{hiddenCount}"
                : firstDef.LabelCap;
            _tooltip = string.Join("\n", orderedDefs.Select(def => def.LabelCap));
            List<string> expandedLines = orderedDefs
                .Take(1)
                .Select(def => def.LabelCap.ToString())
                .ToList();
            if (orderedDefs.Count > expandedLines.Count)
            {
                expandedLines.Add($"+{orderedDefs.Count - expandedLines.Count} {Localization.Get(Localization.More)}");
            }
            _expandedText = string.Join("\n", expandedLines);
            Width = expandedLines.Max(line => Verse.Text.CalcSize(line).x);
        }
    }

    public void Draw(Rect rect)
    {
        if (Text != null)
        {
            string displayText = MultiValueDisplay.IsExpanded ? _expandedText ?? Text : Text;
            rect
                .Label(displayText, GUIStyles.TableCell.String)
                .Tip(_tooltip);
        }
    }
}
