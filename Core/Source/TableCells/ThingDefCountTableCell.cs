using UnityEngine;
using Verse;

namespace Stats.TableCells;

public interface IThingDefCountTableCell : ITableCell
{
    public ThingDef? ThingDef { get; }
    public string ThingDefLabel { get; }
    public decimal Count { get; }
}

public readonly struct ThingDefCountTableCell : IThingDefCountTableCell
{
    public float Width { get; }
    public ThingDef? ThingDef { get; }
    public string ThingDefLabel { get; } = "";
    public decimal Count { get; }

    public ThingDefCountTableCell(ThingDef thingDef, decimal count)
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
        if (ThingDef != null && Event.current.type == EventType.Repaint)
        {
            rect = rect.ContractedByObjectTableCellPadding();

            TextAnchor textAnchor = Text.Anchor;
            Text.Anchor = (TextAnchor)TableCellStyleType.Number;

            Verse.Widgets.Label(rect, "TODO");

            Text.Anchor = textAnchor;
        }
    }
}
