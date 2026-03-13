using System.Collections.Generic;
using System.Linq;
using Stats.Utils;
using UnityEngine;

namespace Stats.ColumnWorkers.Cells;

public interface IDefSetTableCell : ITableCell
{
    public IReadOnlyCollection<Verse.Def>? Value { get; }
    public string? Text { get; }
}

// TODO: Since all rows are now of constant height, we need to refactor this cell.
public readonly struct DefSetTableCell : IDefSetTableCell
{
    public float Width { get; }
    public bool IsRefreshable => false;
    public IReadOnlyCollection<Verse.Def>? Value { get; }
    public string? Text { get; }

    public DefSetTableCell(IReadOnlyCollection<Verse.Def> value)
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
        if (Text != null)
        {
            rect.Label(Text, GUIStyles.TableCell.String);
        }
    }
}
