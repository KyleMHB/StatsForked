using System;
using System.Collections.Generic;
using System.Linq;
using Stats.ObjectTable.FilterWidgets;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats.ObjectTable.Cells;

public readonly struct ThingDefSetCell : ICell
{
    public float Width { get; }
    public readonly IReadOnlyCollection<ThingDef?> Value;

    public ThingDefSetCell(IReadOnlyCollection<ThingDef?> value)
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

    //static private IReadOnlyCollection<ThingDef?> GetValue(Cell cell)
    //{
    //    return ((ThingDefSetCell)cell).Value;
    //}
    //static private int CompareByCellText(Cell cell1, Cell cell2)
    //{
    //    return ((ThingDefSetCell)cell1).Text.CompareTo(((ThingDefSetCell)cell2).Text);
    //}
    //static public CellDescriptor GetDescriptor(ColumnDef columnDef, IEnumerable<ThingDef?> defs) => GetDescriptor(columnDef.Title, defs);
    //static public CellDescriptor GetDescriptor(Widget valueFieldLabel, IEnumerable<ThingDef?> defs)
    //{
    //    IEnumerable<NTMFilterOption<ThingDef?>> valueFieldFilterOptions = defs
    //        .OrderBy(def => def?.label)
    //        .Select<ThingDef?, NTMFilterOption<ThingDef?>>(
    //            def => def == null ? new() : new(def, def.LabelCap, new ThingDefIcon(def))
    //        );
    //    FilterWidget valueFieldFilter = new MTMFilter<ThingDef?>(GetValue, valueFieldFilterOptions);
    //    CellFieldDescriptor valueField = new(valueFieldLabel, valueFieldFilter, CompareByCellText);

    //    return new CellDescriptor(CellStyleType.String, [valueField]);
    //}
}
