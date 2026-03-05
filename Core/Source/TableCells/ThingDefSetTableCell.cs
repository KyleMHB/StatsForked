using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats.TableCells;

public interface IThingDefSetTableCell : ITableCell
{
    public IReadOnlyCollection<ThingDef?> Value { get; }
}

public readonly struct ThingDefSetTableCell : IThingDefSetTableCell
{
    public float Width { get; }
    public IReadOnlyCollection<ThingDef?> Value { get; }

    public ThingDefSetTableCell(IReadOnlyCollection<ThingDef?> value)
    {
        Value = value;
    }

    public void Draw(Rect rect)
    {
        if (Value.Count > 0 && Event.current.type == EventType.Repaint)
        {
            rect = rect.ContractedByObjectTableCellPadding();

            Verse.Widgets.Label(rect, "TODO");
        }
    }
}
