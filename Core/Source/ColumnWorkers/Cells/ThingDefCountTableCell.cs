using Stats.Extensions;
using UnityEngine;

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

    public ThingDefCountTableCell(Verse.ThingDef thingDef, decimal count)
    {
        ThingDef = thingDef;
        if (thingDef != null)
        {
            ThingDefLabel = thingDef.label;
        }
        Count = count;
    }

    public void Draw(Rect rect)
    {
        if (ThingDef != null)
        {
            rect = rect.ContractedByObjectTableCellPadding();
            Widgets_Legacy.Draw.Label(rect, "TODO", GUISkin.TableCell.String);
        }
    }
}
