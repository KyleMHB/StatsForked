using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using RimWorld;
using Stats.FilterWidgets;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats.TableCells;

public readonly struct DefTableCell : ITableCell
{
    public float Width { get; }
    public readonly Def? Value;
    public readonly string Text = "";

    public DefTableCell(Def value)
    {
        Value = value;
        Text = value.LabelCap;
        Width = Verse.Text.CalcSize(Text).x;
    }

    public void Draw(Rect rect)
    {
        if (Value != null && Event.current.type == EventType.Repaint)
        {
            rect = rect.ContractedByObjectTableCellPadding();

            TextAnchor textAnchor = Verse.Text.Anchor;
            Verse.Text.Anchor = (TextAnchor)TableCellStyleType.String;

            Verse.Widgets.Label(rect, Text);

            Verse.Text.Anchor = textAnchor;
        }
    }

    //static private int CompareByDefLabel(Cell cell1, Cell cell2)
    //{
    //    return GetCellText(cell1).CompareTo(GetCellText(cell2));
    //}

    //static public CellDescriptor GetDescriptor(ColumnDef columnDef, IEnumerable<Def?> defs) => GetDescriptor(columnDef.Title, defs);

    //static public CellDescriptor GetDescriptor(Widget valueFieldLabel, IEnumerable<Def?> defs)
    //{
    //    IEnumerable<NTMFilterOption<Def?>> valueFieldFilterOptions = defs
    //        .OrderBy(def => def?.label)
    //        .Select<Def?, NTMFilterOption<Def?>>(
    //            def => def == null ? new() : new(def, def.LabelCap)
    //        );
    //    FilterWidget valueFieldFilter = new OTMFilter<Def?>(GetValue, valueFieldFilterOptions);
    //    CellFieldDescriptor valueField = new(valueFieldLabel, valueFieldFilter, CompareByDefLabel);

    //    return new CellDescriptor(CellStyleType.String, [valueField]);
    //}
}
