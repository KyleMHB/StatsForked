using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Stats.TableCells;

public interface IDefSetTableCell : ITableCell
{
    public IReadOnlyCollection<Def> Value { get; }
    public string Text { get; }
}

// TODO: Since all rows are now of constant height, we need to refactor this cell.
public readonly struct DefSetTableCell : IDefSetTableCell
{
    private static readonly HashSet<Def> EmptyDefHashSet = [];

    public float Width { get; }
    public bool IsRefreshable => false;
    public IReadOnlyCollection<Def> Value { get; } = EmptyDefHashSet;
    public string Text { get; } = "";

    public DefSetTableCell(IReadOnlyCollection<Def> value)
    {
        Value = value;
        if (value.Count > 0)
        {
            Text = string.Join("\n", Value.Select(def => def.LabelCap).OrderBy(text => text));
            Width = Verse.Text.CalcSize(Text).x;
        }
    }

    public void Draw(Rect rect)
    {
        if (Text.Length > 0 && Event.current.type == EventType.Repaint)
        {
            rect = rect.ContractedByObjectTableCellPadding();

            TextAnchor textAnchor = Verse.Text.Anchor;
            Verse.Text.Anchor = (TextAnchor)TableCellStyleType.String;

            Verse.Widgets.Label(rect, Text);

            Verse.Text.Anchor = textAnchor;
        }
    }
}
