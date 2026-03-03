using System;
using System.Collections.Generic;
using System.Linq;
using Stats.FilterWidgets;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats.TableCells;

public readonly struct ThingDefSetTableCell : ITableCell
{
    public float Width { get; }
    public readonly IReadOnlyCollection<ThingDef?> Value;

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
