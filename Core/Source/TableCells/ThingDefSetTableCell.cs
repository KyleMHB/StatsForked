using System.Collections.Generic;
using Stats.Extensions;
using UnityEngine;
using Verse;

namespace Stats.TableCells;

public interface IThingDefSetTableCell : ITableCell
{
    public IReadOnlyCollection<ThingDef?>? Value { get; }
}

public readonly struct ThingDefSetTableCell : IThingDefSetTableCell
{
    public float Width { get; }
    public bool IsRefreshable => false;
    public IReadOnlyCollection<ThingDef?>? Value { get; }

    public ThingDefSetTableCell(IReadOnlyCollection<ThingDef?> value)
    {
        Value = value;
    }

    public void Draw(Rect rect)
    {
        if (Value?.Count > 0)
        {
            rect = rect.ContractedByObjectTableCellPadding();
            Widgets_Legacy.Draw.Label(rect, "TODO", TableCellStyle.String);
        }
    }
}
