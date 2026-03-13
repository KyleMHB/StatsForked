using System.Collections.Generic;
using Stats.Utils;
using UnityEngine;

namespace Stats.ColumnWorkers.Cells;

public interface IThingDefSetTableCell : ITableCell
{
    public IReadOnlyCollection<Verse.ThingDef?>? Value { get; }
}

public readonly struct ThingDefSetTableCell : IThingDefSetTableCell
{
    public float Width { get; }
    public bool IsRefreshable => false;
    public IReadOnlyCollection<Verse.ThingDef?>? Value { get; }

    public ThingDefSetTableCell(IReadOnlyCollection<Verse.ThingDef?> value)
    {
        Value = value;
    }

    public void Draw(Rect rect)
    {
        if (Value?.Count > 0)
        {
            rect.Label("TODO", GUIStyles.TableCell.String);
        }
    }
}
